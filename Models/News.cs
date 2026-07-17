using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDNews { get; set; }
        public string? UrlGambar { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? Judul { get; set; }

        // Kolom Multi-Bahasa
        public string? Judul_ID { get; set; }
        public string? Judul_EN { get; set; }
        public string? IsiContent_ID { get; set; }
        public string? IsiContent_EN { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsSent { get; set; } = false; // Untuk melacak apakah sudah dibroadcast

        //Properti Navigasi
        [ForeignKey(nameof(IDKategori))]
        public int IDKategori { get; set; }
        public virtual Kategori? Kategori { get; set; }        

    }
}