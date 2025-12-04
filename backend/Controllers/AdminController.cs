using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        public class CreateEmployeeRequest
        {
            public string FullName { get; set; } = null!;
            public string? PhoneNo { get; set; }
            public string? Email { get; set; }
            public string Position { get; set; } = null!;
            public int DepartmentId { get; set; }
            public string Status { get; set; } = "Active"; // Default: Active employee
            public DateTime HireDate { get; set; }
        }

        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            // totals
            var totalDepartments = _db.Departments.Count();
            var totalEmployees   = _db.Employees.Count();

            // find RoleId for "Manager"
            int? managerRoleId = _db.Roles
                .Where(r => r.RoleName == "Manager")
                .Select(r => (int?)r.RoleId)
                .FirstOrDefault();

            int totalManagers = 0;
            if (managerRoleId.HasValue)
            {
                // count users who are Managers, are active, and actually mapped to an employee
                totalManagers = _db.Users.Count(u =>
                    u.RoleId == managerRoleId.Value &&
                    u.IsActive &&
                    u.EmployeeId != null);
            }

            return Ok(new
            {
                totalDepartments,
                totalEmployees,
                totalManagers
            });
        }


        [HttpPost("employees")]
        public IActionResult CreateEmployee(CreateEmployeeRequest request)
        {
            // Validate unique email if required
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                bool emailExists = _db.Employees.Any(e => e.Email == request.Email);
                if (emailExists)
                    return BadRequest("Employee with this email already exists.");
            }

            var employee = new Employee
            {
                FullName = request.FullName.Trim(),
                PhoneNo = request.PhoneNo,
                Email = request.Email?.ToLower(),
                Position = request.Position.Trim(),
                DepartmentId = request.DepartmentId,
                Status = request.Status,
                HireDate = request.HireDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Employees.Add(employee);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Employee created successfully",
                employeeId = employee.EmployeeId
            });
        }

    
    }
}