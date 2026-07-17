using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Testimonial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDTestimonial { get; set; }

        //menampung filter multi bahasa
        [NotMapped]
        public string? IsiTestimoni { get; set; }
        
        //menampung filter multi bahasa
        [NotMapped]
        public string? Jabatan { get; set; }

        public string? UrlFoto { get; set; }
        public string? Nama { get; set; }
        // Kolom Multi-Bahasa
        public string? Jabatan_ID { get; set; }
        public string? Jabatan_EN { get; set; }
        public string? IsiTestimoni_ID { get; set; }
        public string? IsiTestimoni_EN { get; set; }
        public DateTime CreateAt { get; set; }
    }
}