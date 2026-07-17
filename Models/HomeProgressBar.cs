using System.ComponentModel.DataAnnotations.Schema;
using company_profile;

public class HomeProgressBar
{
    public int Id { get; set; }
    public int Value { get; set; } // Contoh: 95 (Gunakan int agar lebih mudah divalidasi di HTML/Backend)

    //menampung filter multi-bahasa
    [NotMapped]
    public string Judul { get; set; } = null!;

    // Kolom Multi-Bahasa
    public string Judul_ID { get; set; } = null!;
    public string Judul_EN { get; set; } = null!;

    // Foreign Key ke tabel Home
    public int HomeId { get; set; }
    public virtual Home Home { get; set; } = null!;
}