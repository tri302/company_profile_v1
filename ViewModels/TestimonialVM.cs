using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class TestimonialVM
    {
        public int IDTestimonial { get; set; }

         //menampung filter multi bahasa
        [NotMapped]
        [ValidateNever]
        public string? IsiTestimoni { get; set; }
        
        //menampung filter multi bahasa
        [NotMapped]
        [ValidateNever]
        public string? Jabatan { get; set; }

        public string? UrlGambar { get; set; }
        public IFormFile? UploadGambar { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        [Display(Name = "Nama")]
        public string Nama { get; set; } = string.Empty;

        // Kolom Multi-Bahasa
        [Display(Name = "Jabatan (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Jabatan (Bahasa Indonesia) wajib diisi")]
        public string? Jabatan_ID { get; set; }
        
        [Display(Name = "Jabatan (Bahasa Inggris)")]
        [Required(ErrorMessage = "Jabatan (Bahasa Inggris) wajib diisi")]
        public string? Jabatan_EN { get; set; }

        [Display(Name = "Isi Testimoni (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Isi Testimoni (Bahasa Indonesia) wajib diisi")]
        public string? IsiTestimoni_ID { get; set; }

        [Display(Name = "Isi Testimoni (Bahasa Inggris)")]
        [Required(ErrorMessage = "Isi Testimoni (Bahasa Inggris) wajib diisi")]
        public string? IsiTestimoni_EN { get; set; }
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        public int TotalPages { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public List<TestimonialVM> List { get; set; } = new List<TestimonialVM>();
    }
}