using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using Isopoh.Cryptography.Argon2;
using backend.Models;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public LoginController(IConfiguration config, AppDbContext db)
    {
        _config = config;
        _db = db;
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    private void SaveRefreshToken(int userId, string token)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7) // refresh token lifetime
        };

        _db.RefreshTokens.Add(refreshToken);
        _db.SaveChanges();
    }

    [HttpGet("hash/{password}")]
    public IActionResult GenerateHash(string password)
    {
        string hash = Argon2.Hash(password);
        return Ok(hash);
    }

    [HttpPost("auth")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Normalize username input to avoid invisible login bugs
        string normalizedUsername = request.Username.Trim().ToLowerInvariant();

        // Fetch user and role WITHOUT checking password
        var userWithRole = (from u in _db.Users
                    join r in _db.Roles on u.RoleId equals r.RoleId
                    where u.Username.ToLower() == normalizedUsername
                          && u.IsActive == true
                    select new 
                    { 
                        User = u, 
                        RoleName = r.RoleName 
                    })
                    .FirstOrDefault();

        if (userWithRole == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var user = userWithRole.User;

        // Verify hash instead of comparing plaintext
        bool validPassword = Argon2.Verify(user.Password, request.Password);

        if (!validPassword)
            return Unauthorized(new { message = "Invalid credentials" });

        // Generate JWT (same as before)
        var roleName = userWithRole.RoleName;
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("email", user.Email),
            new Claim(ClaimTypes.Role, roleName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        var refreshToken = GenerateRefreshToken();
        SaveRefreshToken(user.UserId, refreshToken);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = refreshToken,
            user = new
            {
                id = user.UserId,
                username = user.Username,
                email = user.Email,
                role = roleName
            }
        });
    }

        [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        var storedToken = _db.RefreshTokens
            .FirstOrDefault(rt => rt.Token == request.RefreshToken && rt.RevokedAt == null);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        var user = _db.Users.Find(storedToken.UserId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        // generate new JWT
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("email", user.Email),
            new Claim(ClaimTypes.Role, _db.Roles.Find(user.RoleId)!.RoleName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    public record RefreshRequest(string RefreshToken);

    [HttpPost("logout")]
    public IActionResult Logout([FromBody] RefreshRequest request)
    {
        var storedToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
        if (storedToken != null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            _db.SaveChanges();
        }

        return Ok(new { message = "Logged out" });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            username = User.Identity?.Name,
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

}

public record LoginRequest(string Username, string Password);
