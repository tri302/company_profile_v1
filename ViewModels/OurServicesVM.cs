using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile
{
    public class OurServicesVM
    {
        public int IDOurServices { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiService { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulService { get; set; }

        // Kolom Multi-Bahasa
        [Display(Name = "Judul Service (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Judul Service (Bahasa Indonesia) wajib diisi.")]
        public string? JudulService_ID { get; set; }
        
        [Display(Name = "Judul Service (Bahasa Inggris)")]
        [Required(ErrorMessage = "Judul Service (Bahasa Inggris) wajib diisi.")]
        public string? JudulService_EN { get; set; }

        [Display(Name = "Isi Service (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Isi Service (Bahasa Indonesia) wajib diisi.")]
        public string? IsiService_ID { get; set; }

        [Display(Name = "Isi Service (Bahasa Inggris)")]
        [Required(ErrorMessage = "Isi Service (Bahasa Inggris) wajib diisi.")]
        public string? IsiService_EN { get; set; }
        public string? UrlGambar { get; set; }
        public IFormFile? UploadGambar { get; set; }
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        public List<OurServicesVM> ListOurServices { get; set; } = new List<OurServicesVM>();

        [ValidateNever]
        public ContactFormVM ContactForm { get; set; } = new ContactFormVM();

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public int TotalPages { get; set; }
    }
}