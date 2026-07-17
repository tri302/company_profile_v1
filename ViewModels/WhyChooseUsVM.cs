using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile.ViewModels
{
    public class WhyChooseUsVM
    {
        public int Id { get; set; }

        //menampung filter multi bahasa
        [NotMapped]
        [ValidateNever]
        public string? JudulWhyChooseUs { get; set; }

        [NotMapped]
        //menampung filter multi bahasa 
        [ValidateNever]
        public string? IsiContentWhyChooseUs { get; set; }

        // Kolom Multi-Bahasa
        [Display(Name = "Judul Why Choose Us (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Wajib di isi")]
        public string JudulWhyChooseUs_ID { get; set; } = null!;

        [Display(Name = "Judul Why Choose Us (Bahasa Inggris)")]
        [Required(ErrorMessage = "Wajib di isi")]
        public string JudulWhyChooseUs_EN { get; set; } = null!;

        [Display(Name = "Isi Content Why Choose Us (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Wajib di isi")]
        public string IsiContentWhyChooseUs_ID { get; set; } = null!;

        [Display(Name = "Isi Content Why Choose Us (Bahasa Inggris)")]
        [Required(ErrorMessage = "Wajib di isi")]
        public string IsiContentWhyChooseUs_EN { get; set; } = null!;

        [Display(Name = "URL Gambar")]        
        public string UrlGambar { get; set; } = string.Empty;

        public IFormFile? UploadGambar { get; set; }
    }
}
