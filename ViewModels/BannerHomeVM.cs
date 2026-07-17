using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile.ViewModels
{
    public class BannerHomeVM
    {        
        public int Id { get; set; }    

        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulTextAnimatedOnBanner { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiTextAnimatedOnBanner { get; set; }

        [Required(ErrorMessage = "Wajib diisi.")]                    
        // Kolom Multi-Bahasa
        public string JudulTextAnimatedOnBanner_ID { get; set; } = null!;

        [Required(ErrorMessage = "Wajib diisi.")]
        public string JudulTextAnimatedOnBanner_EN { get; set; } = null!;

        [Required(ErrorMessage = "Wajib diisi.")]
        public string IsiTextAnimatedOnBanner_ID { get; set; } = null!;

        [Required(ErrorMessage = "Wajib diisi.")]
        public string IsiTextAnimatedOnBanner_EN { get; set; } = null!;


        public string UrlGambar { get; set; } = string.Empty;   
        public IFormFile? file { get; set; }
    }
}