using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class TeamVM
    {
        public int IDTeam { get; set; }

        //menampung filter multi bahasa        
        [NotMapped]
        [ValidateNever]
        public string? Jabatan { get; set; }

        //menampung filter multi bahasa        
        [NotMapped]
        [ValidateNever]
        public string? Deskripsi { get; set; }

        public string? UrlGambar { get; set; }
    
        public IFormFile? UploadGambar { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        [Display(Name = "Nama")]
        public string Nama { get; set; } = string.Empty;

        // Kolom Multi-Bahasa
        [Display(Name = "Jabatan (ID)")]
        [Required(ErrorMessage = "Jabatan (Bahasa Indonesia) wajib diisi")]
        public string? Jabatan_ID { get; set; }
        
        [Display(Name = "Jabatan (EN)")]
        [Required(ErrorMessage = "Jabatan (Bahasa Inggris) wajib diisi")]
        public string? Jabatan_EN { get; set; }

        [Display(Name = "Deskripsi (ID)")]
        [Required(ErrorMessage = "Deskripsi (Bahasa Indonesia) wajib diisi")]
        public string Deskripsi_ID { get; set; }

        [Display(Name = "Deskripsi (EN)")]
        [Required(ErrorMessage = "Deskripsi (Bahasa Inggris) wajib diisi")]
        public string Deskripsi_EN { get; set; }

        public string? UrlFb { get; set; }
        public string? UrlLinkedIn { get; set; }
        public string? UrlYoutube { get; set; }
        public string? UrlTwiter { get; set; }

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Display(Name = "Nomor HP")]
        [Phone(ErrorMessage = "Format nomor HP tidak valid")]
        [Required(ErrorMessage = "Nomor HP wajib diisi")]
        public string? NoHP { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [ValidateNever]
        public int TotalPages { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public List<TeamVM> List { get; set; } = new List<TeamVM>();
    }
}