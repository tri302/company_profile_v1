using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class PricingVM
    {
        public int IDPricing { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? JenisLayanan { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? DeskripsiJenisLayanan { get; set; }

        // Kolom Multi-Bahasa
        [Display(Name = "Jenis Layanan dalam bahasa Indonesia")]
        [Required(ErrorMessage = "Jenis Layanan dalam bahasa Indonesia wajib diisi (ID)")]
        public string? JenisLayanan_ID { get; set; }

        [Display(Name = "Jenis Layanan dalam bahasa Inggris")]
        [Required(ErrorMessage = "Jenis Layanan dalam bahasa Inggris wajib diisi (EN)")]
        public string? JenisLayanan_EN { get; set; }

        [Display(Name = "Deskripsi Jenis Layanan dalam bahasa Indonesia")]
        [Required(ErrorMessage = "Deskripsi Jenis Layanan dalam bahasa Indonesia wajib diisi (ID)")]
        public string? DeskripsiJenisLayanan_ID { get; set; }
        
        [Display(Name = "Deskripsi Jenis Layanan dalam bahasa Inggris")]
        [Required(ErrorMessage = "Deskripsi Jenis Layanan dalam bahasa Inggris wajib diisi (EN)")]
        public string? DeskripsiJenisLayanan_EN { get; set; }

        [Display(Name = "Harga")]
        [Required(ErrorMessage = "Harga wajib diisi")]
        [Column(TypeName = "decimal(18,2)")]
        public double Harga { get; set; } = 0;
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        public int TotalPages { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public List<PricingVM> List { get; set; } = new List<PricingVM>();
    }
}