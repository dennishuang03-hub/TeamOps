using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("audit_logs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // primary key

        [Required]
        [Column("User_Id")]
        public int UserId { get; set; } // FK referencing Users table

        [MaxLength(100)]
        public string? Module { get; set; } // e.g. "Employee", "Tasks", "Documents"

        [MaxLength(100)]
        public string? Action { get; set; } // e.g. "Create", "Update", "Delete", "Login"

        public string? OldValue { get; set; } // JSON or text of previous record

        public string? NewValue { get; set; } // JSON or text of new record

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; } // auto-set by DB

        // Navigation property for the user who performed the action
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
