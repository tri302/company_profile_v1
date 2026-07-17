using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class BannerHome
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulTextAnimatedOnBanner { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiTextAnimatedOnBanner { get; set; }

        // Kolom Multi-Bahasa
        public string JudulTextAnimatedOnBanner_ID { get; set; } = null!;
        public string JudulTextAnimatedOnBanner_EN { get; set; } = null!;
        public string IsiTextAnimatedOnBanner_ID { get; set; } = null!;
        public string IsiTextAnimatedOnBanner_EN { get; set; } = null!;
        public string UrlGambar { get; set; } = string.Empty;
    }
}