using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Isopoh.Cryptography.Argon2;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly AppDbContext _db;

         public SuperAdminController(AppDbContext db)
        {
            _db = db;
        }

        public class CreateUserRequest
        {
            public int? EmployeeId { get; set; }
            public string Username { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!; // plain text input
            public int RoleId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class UpdateUserRequest
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public bool? IsActive { get; set; }
            public int? RoleId { get; set; }
        }

        public class AssignRoleRequest
        {
            public int RoleId { get; set; }
        }

        // Dashboard Summary count ()
        // ########################### TESTED ############################
        [HttpGet("summary")]
        public IActionResult SummaryCount()
        {
            var summary = new
            {
                totalUsers = _db.Users.Count(),
                totalDepartments = _db.Departments.Count(),
                totalEmployees = _db.Employees.Count()
            };

            return Ok(summary);
        }

        // List All Users
        // ########################### TESTED ############################
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _db.Users
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Email,
                    x.IsActive,
                    Role = x.Role != null ? x.Role.RoleName : "No Role Assigned"
                })
                .ToList();

            return Ok(users);
        }

        // Read Specific User
        // ########################### TESTED ############################
        [HttpGet("users/{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _db.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    Role = u.Role.RoleName,
                    u.EmployeeId
                }).FirstOrDefault();

            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // Create User
        // ########################### TESTED ############################
        [HttpPost("users")]
        public IActionResult CreateUser(CreateUserRequest request)
        {
            string passwordHash = Argon2.Hash(request.Password);

            var user = new User
            {
                EmployeeId = request.EmployeeId, // optional FK
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                Password = passwordHash, // hashed, NOT plain text
                RoleId = request.RoleId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok("User created successfully");
        }

        // Update users
        // ########################### TESTED ############################
        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, UpdateUserRequest request)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound("User not found");

            if (!string.IsNullOrWhiteSpace(request.Username))
                user.Username = request.Username.Trim();

            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email.Trim().ToLower();

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            if (request.RoleId.HasValue)
                user.RoleId = request.RoleId.Value;

            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Ok("User updated successfully");
        }

        // Delete User
        // ########################### TESTED ############################
        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound("User not found");

            _db.Users.Remove(user);
            _db.SaveChanges();

            return Ok("User deleted successfully");
        }

        // Deactivate User
        // ########################### TESTED ############################
        [HttpPatch("users/{id}/deactivate")]
        public IActionResult DeactivateUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound("User not found");

            user.IsActive = false;
            _db.SaveChanges();

            return Ok("User deactivated");
        }

        // Activate User
        // ########################### TESTED ############################
        [HttpPatch("users/{id}/activate")]
        public IActionResult ActivateUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound("User not found");

            user.IsActive = true;
            _db.SaveChanges();
            return Ok("User activated");
        }


        // View Roles
        // ########################### TESTED ############################
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(_db.Roles.ToList());
        }

        // Assign Role To User
        [HttpPatch("users/{id}/role")]
        public IActionResult ReassignRole(int id, AssignRoleRequest request)
        {
            var user = _db.Users.Find(id);
            var role = _db.Roles.Find(request.RoleId);

            if (user == null) return NotFound("User not found");
            if (role == null) return NotFound("Role not found");

            user.RoleId = request.RoleId;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Ok($"Role '{role.RoleName}' assigned to user '{user.Username}'");
        }

    }
}