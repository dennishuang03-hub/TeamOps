using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("licensetypes")]
    public class LicenseType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LicenseTypeId { get; set; }  // Primary key

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!; // Unique license name

        public string? Description { get; set; } // Optional description
    }
}
