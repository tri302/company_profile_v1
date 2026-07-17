using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class PageBannerSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PageName { get; set; } = null!;        
        public string? BannerPath { get; set; } // Menyimpan path gambar banner

        // Digunakan untuk proses upload file baru
        [NotMapped]
        public IFormFile? NewBanner { get; set; }
    }
}