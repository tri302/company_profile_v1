using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class OurServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDOurServices { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiService { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulService { get; set; }

        // Kolom Multi-Bahasa
        public string? JudulService_ID { get; set; }
        public string? JudulService_EN { get; set; }
        public string? IsiService_ID { get; set; }
        public string? IsiService_EN { get; set; }
        public string? UrlGambar { get; set; }
        public DateTime CreateAt { get; set; }
    }
}