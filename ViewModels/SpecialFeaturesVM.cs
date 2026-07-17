using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SpecialFeaturesVM
{
    public int Id { get; set; }

    //untuk menampung filter multi-bahasa
    [NotMapped]
    [ValidateNever]
    public string? JudulSpecialFeatures { get; set; }

    //untuk menampung filter multi-bahasa
    [NotMapped]
    [ValidateNever]
    public string? IsiContentSpecialFeatures { get; set; }

    // Kolom Multi-Bahasa
    [Display(Name = "Judul Special Features (ID)")]
    [Required(ErrorMessage = "Judul Special Features dalam bahasa Indonesia wajib diisi (ID)")]
    public string JudulSpecialFeatures_ID { get; set; } = null!;

    [Display(Name = "Judul Special Features (EN)")]
    [Required(ErrorMessage = "Judul Special Features dalam bahasa Inggris wajib diisi (EN)")]
    public string JudulSpecialFeatures_EN { get; set; } = null!;

    [Display(Name = "Isi Content Special Features (ID)")]
    [Required(ErrorMessage = "Isi Content Special Features dalam bahasa Indonesia wajib diisi (ID)")]
    public string IsiContentSpecialFeatures_ID { get; set; } = null!;
    
    [Display(Name = "Isi Content Special Features (EN)")]
    [Required(ErrorMessage = "Isi Content Special Features dalam bahasa Inggris wajib diisi (EN)")]
    public string IsiContentSpecialFeatures_EN { get; set; } = null!;
}