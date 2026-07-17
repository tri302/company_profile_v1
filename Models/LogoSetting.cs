using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class LogoSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Menyimpan path string ke database
        public string? UrlGambar { get; set; }        
        public string? Tipe { get; set; } // Logo atau Favicon

        // Digunakan hanya untuk menerima upload file dari form (tidak masuk DB)
        [NotMapped]
        public IFormFile? NewLogo { get; set; }
        [NotMapped]
        public IFormFile? NewFavicon { get; set; }
    }
}