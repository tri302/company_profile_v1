using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile
{
    public class NewsVM
    {
        public int IDNews { get; set; }       
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? Judul { get; set; }
         
        // Kolom Multi-Bahasa
        [Display(Name = "Judul Berita (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Judul Berita (Bahasa Indonesia) wajib diisi")]
        public string? Judul_ID { get; set; }

        [Display(Name = "Judul Berita (Bahasa Inggris)")]
        [Required(ErrorMessage = "Judul Berita (Bahasa Inggris) wajib diisi")]
        public string? Judul_EN { get; set; }

        [Display(Name = "Isi Berita (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Isi Berita (Bahasa Indonesia) wajib diisi")]
        public string? IsiContent_ID { get; set; }

        [Display(Name = "Isi Berita (Bahasa Inggris)")]
        [Required(ErrorMessage = "Isi Berita (Bahasa Inggris) wajib diisi")]
        public string? IsiContent_EN { get; set; }
        public string? UrlGambar { get; set; }
        public IFormFile? UploadGambar { get; set; }        
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }
        [ValidateNever]
        public int TotalPages { get; set; }

        // Untuk menapilkan list kategori berita
        // TAMBAHKAN INI: Untuk menampung ID yang dipilih dari View/Form
        // [Required(ErrorMessage = "Kategori wajib dipilih")]
        // [Display(Name = "Kategori")]
        [ValidateNever]
        public int IDKategori { get; set; }

        // Properti tambahan untuk tampilan (Read-Only)
        [ValidateNever]
        public string? NamaKategori { get; set; }       

        [ValidateNever]
        public KategoriVM? KategoriBerita { get; set; }

        [ValidateNever]
        public List<NewsVM>? ListBerita { get; set; } = new List<NewsVM>();

        [ValidateNever]
        public List<NewsVM>? ListRecentPostsBerita { get; set; } = new List<NewsVM>();

        [ValidateNever]
        public List<KategoriVM> ListKategoriBerita { get; set; }

        [ValidateNever]
        public Kategori? Kategori { get; set; }               

    }
}