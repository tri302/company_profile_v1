using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class WhyChooseUs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? JudulWhyChooseUs { get; set; } 

        [NotMapped]
        //menampung filter multi bahasa 
        public string? IsiContentWhyChooseUs { get; set; } 
        
        // Kolom Multi-Bahasa
        public string JudulWhyChooseUs_ID { get; set; } = null!;
        public string JudulWhyChooseUs_EN { get; set; } = null!;
        public string IsiContentWhyChooseUs_ID { get; set; } = null!;
        public string IsiContentWhyChooseUs_EN { get; set; } = null!;
        public string UrlGambar { get; set; } = string.Empty;
    }
}