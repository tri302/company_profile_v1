using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class FaqVM
    {
        public int IDFaq { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulFaq { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiFaq { get; set; }

        // Kolom Multi-Bahasa

        [Required(ErrorMessage = "Judul FAQ dalam Bahasa Indonesia wajib diisi.")]
        [Display(Name = "Judul FAQ (Bahasa Indonesia)")]
        public string? JudulFaq_ID { get; set; }

        [Required(ErrorMessage = "Judul FAQ dalam Bahasa Inggris wajib diisi.")]
        [Display(Name = "Judul FAQ (Bahasa Inggris)")]
        public string? JudulFaq_EN { get; set; }

        [Required(ErrorMessage = "Isi FAQ dalam Bahasa Indonesia wajib diisi.")]
        [Display(Name = "Isi FAQ (Bahasa Indonesia)")]
        public string? IsiFaq_ID { get; set; }

        [Required(ErrorMessage = "Isi FAQ dalam Bahasa Inggris wajib diisi.")]
        [Display(Name = "Isi FAQ (Bahasa Inggris)")]
        public string? IsiFaq_EN { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [NotMapped]
        [ValidateNever]
        public IEnumerable<FaqVM>? ListFaqs { get; set; } = new List<FaqVM>();
        [ValidateNever]
        public int CurrentPage { get; set; }
        [ValidateNever]
        public int TotalPages { get; set; }
    }
}