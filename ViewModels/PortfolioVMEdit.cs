using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile
{
    public class PortfolioVMEdit
    {
        public int IDPortfolio { get; set; }
        //menampung filter multi-bahasa
        [NotMapped]
        [ValidateNever]
        public string? Judul { get; set; }

        //menampung filter multi-bahasa
        [NotMapped]
        [ValidateNever]
        public string? IsiContentPortfolio { get; set; }

        //menampung filter multi-bahasa
        [NotMapped]
        [ValidateNever]
        public string? ClientComment { get; set; }

        public string? UrlGambar { get; set; }
        public string? UrlFotoProject { get; set; }
        public IFormFile? UploadGambar { get; set; }
        
        public DateTime CreateAt { get; set; }

        [Display(Name = "Nama Klien")]
        [Required(ErrorMessage = "Nama Klien wajib diisi")]
        public string ClientName { get; set; } = string.Empty;

        [Display(Name = "Nama Perusahaan Klien")]
        [Required(ErrorMessage = "Nama Perusahaan Klien wajib diisi")]
        public string ClientCompanyName { get; set; } = string.Empty;

        [Display(Name = "Tanggal Mulai Proyek")]
        [Required(ErrorMessage = "Tanggal Mulai Proyek wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime ProjectStartDate { get; set; }

        [Display(Name = "Tanggal Selesai Proyek")]
        [Required(ErrorMessage = "Tanggal Selesai Proyek wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime ProjectEndDate { get; set; }

        // Kolom Multi-Bahasa
        [Display(Name = "Judul (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Judul (Bahasa Indonesia) wajib diisi")]
        public string? Judul_ID { get; set; }
        [Display(Name = "Judul (Bahasa Inggris)")]
        [Required(ErrorMessage = "Judul (Bahasa Inggris) wajib diisi")]
        public string? Judul_EN { get; set; }
        [Display(Name = "Isi Content Portfolio (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Isi Content Portfolio (Bahasa Indonesia) wajib diisi")]
        public string? IsiContentPortfolio_ID { get; set; }
        [Display(Name = "Isi Content Portfolio (Bahasa Inggris)")]
        [Required(ErrorMessage = "Isi Content Portfolio (Bahasa Inggris) wajib diisi")]
        public string? IsiContentPortfolio_EN { get; set; }
        [Display(Name = "Komentar Klien (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Komentar Klien (Bahasa Indonesia) wajib diisi")]
        public string ClientComment_ID { get; set; } = null!;

        [Display(Name = "Komentar Klien (Bahasa Inggris)")]
        [Required(ErrorMessage = "Komentar Klien (Bahasa Inggris) wajib diisi")]
        public string ClientComment_EN { get; set; } = null!;

        [ValidateNever]
        public List<Portfolio>? ListPortfolio { get; set; } = new List<Portfolio>();

        [ValidateNever]
        public ContactFormVM? ContactForm { get; set; } = new ContactFormVM();

        [ValidateNever]
        public List<IFormFile>? UploadFotoProject { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public List<PortfolioGambar>? PortfolioGambars { get; set; } = new List<PortfolioGambar>();        
    }
}