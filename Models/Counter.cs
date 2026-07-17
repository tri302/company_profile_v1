using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Counter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulCounter { get; set; }
        // Kolom Multi-Bahasa
        public string JudulCounter_ID { get; set; } = null!;
        public string JudulCounter_EN { get; set; } = null!;
        public int IsiContentCounter { get; set; }
    }
}