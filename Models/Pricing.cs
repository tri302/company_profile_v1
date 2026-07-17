using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Pricing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDPricing { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? JenisLayanan { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? DeskripsiJenisLayanan { get; set; }
        
        // Kolom Multi-Bahasa
        public string? JenisLayanan_ID { get; set; }
        public string? JenisLayanan_EN { get; set; }
        public string? DeskripsiJenisLayanan_ID { get; set; }
        public string? DeskripsiJenisLayanan_EN { get; set; }
        public double? Harga { get; set; }
        public DateTime CreateAt { get; set; }
    }
}