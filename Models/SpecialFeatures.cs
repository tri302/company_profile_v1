using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class SpecialFeatures
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //untuk menampung filter multi-bahasa
        [NotMapped]
        public string? JudulSpecialFeatures { get; set; }
        //untuk menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContentSpecialFeatures { get; set; }
        
        // Kolom Multi-Bahasa
        public string JudulSpecialFeatures_ID { get; set; } = null!;
        public string JudulSpecialFeatures_EN { get; set; } = null!;
        public string IsiContentSpecialFeatures_ID { get; set; } = null!;
        public string IsiContentSpecialFeatures_EN { get; set; } = null!;
    }
}