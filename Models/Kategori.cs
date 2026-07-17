using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Kategori
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDKategori { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? NamaKategori { get; set; }

        // Kolom Multi-Bahasa
        public string? NamaKategori_ID { get; set; }
        public string? NamaKategori_EN { get; set; }

        //Properti Navigasi ke News
        //Agar relasinya terbaca sebagai One-to-Many (Satu kategori bisa punya banyak berita)
        //sebaiknya menambahkan koleksi News di dalam kelas Kategori yang bertipe koleksi.        
        public virtual ICollection<News>? News { get; set; }
    }
}