using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("employeelicenses")]
    public class EmployeeLicense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeLicenseId { get; set; } // PK

        [Required]
        public int EmployeeId { get; set; } // FK → Employees table

        [Required]
        public int LicenseTypeId { get; set; } // FK → LicenseTypes table

        [MaxLength(100)]
        public string? LicenseNumber { get; set; } 

        [Column(TypeName = "date")]
        public DateTime? IssueDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(150)]
        public string? IssuedBy { get; set; } // Optional issuing authority

        // Auto-updated timestamp from database
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(LicenseTypeId))]
        public virtual LicenseType LicenseType { get; set; } = null!;
    }
}
