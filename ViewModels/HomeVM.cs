using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile.ViewModels.HomeVM
{
    public class HomeVM
    {
        public int Id { get; set; }

        //Properti untuk menampung hasil filter multi bahasa dari db
        [NotMapped]
        [ValidateNever]
        public string? JudulAboutStart { get; set; }

        //Properti untuk menampung hasil filter multi bahasa dari db
        [NotMapped]
        [ValidateNever]
        public string? IsiAboutStart { get; set; }

        // Kolom Multi-Bahasa
        [Display(Name = "Judul About Start dalam bahasa Indonesia")]
        [Required(ErrorMessage = "Wajib di isi tidak boleh kosong")]
        public string JudulAboutStart_ID { get; set; } = null!;

        [Display(Name = "Judul About Start dalam bahasa Inggris")]
        [Required(ErrorMessage = "Wajib di isi tidak boleh kosong")]
        public string JudulAboutStart_EN { get; set; } = null!;

        [Display(Name = "Isi About Start dalam bahasa Indonesia")]
        [Required(ErrorMessage = "Wajib di isi tidak boleh kosong")]
        public string IsiAboutStart_ID { get; set; } = null!;

        [Display(Name = "Isi About Start dalam bahasa Inggris")]
        [Required(ErrorMessage = "Wajib di isi tidak boleh kosong")]
        public string IsiAboutStart_EN { get; set; } = null!;

        [Display(Name = "Embeded URL Video")]
        [Required(ErrorMessage = "Wajib di isi tidak boleh kosong")]
        public string EmbededUrlVidio { get; set; } = string.Empty;
        public string? UrlGambar { get; set; }
        //public IFormFile? File { get; set; }


        [ValidateNever]
        public int CurrentPage { get; set; }
        [ValidateNever]
        public int TotalPages { get; set; }

        public List<HomeVM>? List { get; set; } = new List<HomeVM>();

        // Menampung list progress bar dinamis dari form
        [ValidateNever]
        public List<ProgressBarItemVM>? ProgressBars { get; set; } = new List<ProgressBarItemVM>();
    }

    public class ProgressBarItemVM
    {
        public int Id { get; set; } // Penting untuk membedakan item lama vs item baru saat Edit

        //Menampung hasil filter dari db ke view untuk bahasa indonesia dan inggris
        [NotMapped]
        [ValidateNever]
        public string Judul { get; set; } = string.Empty;

        [Required]
        public string Judul_ID { get; set; } = string.Empty;

        [Required]
        public string Judul_EN { get; set; } = string.Empty;

        [Range(0, 100)]
        public int Value { get; set; }
    }
}
