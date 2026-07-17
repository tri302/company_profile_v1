using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Faq
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDFaq { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulFaq { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiFaq { get; set; }

        // Kolom Multi-Bahasa
        public string? JudulFaq_ID { get; set; }
        public string? JudulFaq_EN { get; set; }
        public string? IsiFaq_ID { get; set; }
        public string? IsiFaq_EN { get; set; }
        public DateTime CreateAt { get; set; }
    }
}