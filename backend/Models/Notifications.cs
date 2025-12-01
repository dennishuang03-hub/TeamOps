using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // PK

        [Required]
        [Column("User_Id")]
        public int UserId { get; set; } // FK → Users table

        [MaxLength(255)]
        public string? Title { get; set; } // optional title

        public string? Message { get; set; } // notification body text

        [Column("Is_Read")]
        public bool IsRead { get; set; } = false; // default unread

        [Column("Sent_At")]
        public DateTime SentAt { get; set; } // DB sets this automatically

        // -------------------------
        // Navigation
        // -------------------------
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
