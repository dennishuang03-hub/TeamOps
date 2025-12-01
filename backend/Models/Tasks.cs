using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("tasks")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        // ENUM('Low','Medium','High','Critical') DEFAULT 'Low'
        [Required]
        [MaxLength(10)]
        public string Priority { get; set; } = "Low";

        // ENUM('Pending','In Progress','Completed','Overdue') DEFAULT 'Pending'
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "date")]
        public DateTime? DueDate { get; set; }

        // FK: user who created the task
        [Required]
        public int CreatedBy { get; set; }

        // FK: user assigned to carry out the task
        public int? AssignedTo { get; set; }

        // FK: department this task relates to
        public int? DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // ---------------------------------
        // Navigation Properties
        // ---------------------------------

        // creator of this task
        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByUser { get; set; } = null!;

        // employee assigned to this task (nullable)
        [ForeignKey(nameof(AssignedTo))]
        public virtual User? AssignedToUser { get; set; }

        // relevant department (nullable)
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }
    }
}
