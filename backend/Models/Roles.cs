using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }  

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = null!; 

        public string? Description { get; set; } 
    }
}
