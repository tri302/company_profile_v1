using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class AboutVM
    {
        public int IDAbout { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContent { get; set; }

        [Required(ErrorMessage = "Wajib diisi")]
        [Display(Name = "Isi Content")]
        // Kolom Multi-Bahasa
        public string? IsiContent_ID { get; set; }

         [Required(ErrorMessage = "Wajib diisi")]
        [Display(Name = "Isi Content")]
        // Kolom Multi-Bahasa
        public string? IsiContent_EN { get; set; }

        public DateTime CreateAt { get; set; }
    }
}

