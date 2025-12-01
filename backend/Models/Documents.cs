using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("documents")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  

        [Required]
        [Column("User_Id")]
        public int UserId { get; set; }  

        [Required]
        [MaxLength(255)]
        [Column("File_Name")]
        public string FileName { get; set; } = null!;

        [Required]
        [Column("File_URL")]
        public string FileURL { get; set; } = null!;

        [MaxLength(100)]
        [Column("Doc_Type")]
        public string? DocType { get; set; }  

        [Column("Expiry_Date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [Column("Uploaded_By")]
        public int UploadedBy { get; set; }  

        [Column("Uploaded_At")]
        public DateTime UploadedAt { get; set; }  

        // Document owner
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        // Who uploaded the document
        [ForeignKey(nameof(UploadedBy))]
        public virtual User UploadedByUser { get; set; } = null!;
    }
}
