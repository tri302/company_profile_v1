using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Home
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ValueProgressBar { get; set; } = string.Empty;
        public string EmbededUrlVidio { get; set; } = string.Empty;
        //public string UrlGambar { get; set; } = string.Empty;
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string JudulAboutStart { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string IsiAboutStart { get; set; }

        // Kolom Multi-Bahasa
        public string JudulAboutStart_ID { get; set; } = null!;
        public string JudulAboutStart_EN { get; set; } = null!;
        public string IsiAboutStart_ID { get; set; } = null!;
        public string IsiAboutStart_EN { get; set; } = null!;        

        // Relasi ke tabel anak (Navigation Property)
        public virtual ICollection<HomeProgressBar> ProgressBars { get; set; } = new List<HomeProgressBar>();
    }
}