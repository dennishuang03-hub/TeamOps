using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("departments")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }  // Primary key

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!; // Unique department name

        public string? Description { get; set; } // Optional notes/details
    }
}
