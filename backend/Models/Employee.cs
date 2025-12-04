using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("employees")] // matches your table name
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [StringLength(50)]
        public string? PhoneNo { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Position { get; set; } = null!;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime HireDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; } = null!;
    }
}
