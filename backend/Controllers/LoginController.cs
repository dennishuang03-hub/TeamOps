using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using Isopoh.Cryptography.Argon2;

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

    [HttpGet("hash/{password}")]
    public IActionResult GenerateHash(string password)
    {
        string hash = Argon2.Hash(password);
        return Ok(hash);
    }

    [HttpPost]
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
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            user = new
            {
                id = user.UserId,
                username = user.Username,
                email = user.Email,
                role = roleName
            }
        });
    }

}

public record LoginRequest(string Username, string Password);
