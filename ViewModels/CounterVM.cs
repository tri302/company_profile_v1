using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CounterVM
{
    public int Id { get; set;} 
    
        //menampung filter multi-bahasa
    [NotMapped]
    public string? JudulCounter { get; set; }

    [Required(ErrorMessage = "Wajib di isi !")]
    [Display(Name = "Judul Achievements")]
    // Kolom Multi-Bahasa
    public string JudulCounter_ID { get; set; } = null!;

    [Required(ErrorMessage = "Wajib di isi !")]
    [Display(Name = "Judul Achievements")]
    public string JudulCounter_EN { get; set; } = null!;

    [Required(ErrorMessage = "Wajib di isi !")]
    [Display(Name = "Isi Achievements")] // Hanya berisi angka
    public int IsiContentCounter { get; set;}
}