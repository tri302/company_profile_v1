using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    [Table("newslog")]
    public class NewsLog
    {
        public int ID { get; set; }

        // 🔥 Ini kuncinya! Kasih tahu EF Core kalau properti objek 'News' 
        // merujuknya ke kolom 'IDNews', bukan bikin kolom baru lagi.
        [ForeignKey("IDNews")]
        public int IDNews { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;

        // Relasi (Opsional, agar data rapi)
        public News? News { get; set; }
    }
}