using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Events
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDEvents { get; set; }
        public string? UrlGambar { get; set; } 
        
        //menampung filter multi-bahasa
        [NotMapped]  
        public string? JudulEvent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? AlamatEvent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContentEvent { get; set; } 
        
        //menampung filter multi-bahasa
        [NotMapped] 
        public string? LokasiEvent { get; set; }   
        public DateTime StartEvent { get; set; }
        public DateTime EndEvent { get; set; }

        // Kolom Multi-Bahasa
        public string? JudulEvent_ID { get; set; }
        public string? JudulEvent_EN { get; set; }
        public string? LokasiEvent_ID { get; set; }
        public string? LokasiEvent_EN { get; set; }
        public string? AlamatEvent_ID { get; set; }
        public string? AlamatEvent_EN { get; set; }
        public string? IsiContentEvent_ID { get; set; }
        public string? IsiContentEvent_EN { get; set; }
    }
}