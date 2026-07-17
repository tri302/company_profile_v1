using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class About
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDAbout { get; set; }
        public DateTime CreateAt { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string IsiContent { get; set; }
        
        // Kolom Multi-Bahasa
        public string? IsiContent_ID { get; set; }
        public string? IsiContent_EN { get; set; }
    }
}

