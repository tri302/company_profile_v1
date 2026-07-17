using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class KategoriVM
    {
        public int IDKategori { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? NamaKategori { get; set; }

        // Kolom Multi-Bahasa

        [Display(Name = "Nama Kategori dalam bahasa Indonesia")]
        [Required(ErrorMessage = "Nama kategori dalam bahasa Indonesia wajib diisi.")]
         public string? NamaKategori_ID { get; set; }

        [Display(Name = "Nama Kategori dalam bahasa Inggris")]
        [Required(ErrorMessage = "Nama kategori dalam bahasa Inggris wajib diisi.")]
        public string? NamaKategori_EN { get; set; }

        // Tambahkan ini untuk menampung hitungan saja
        public int TotalNews { get; set; }
    }
}