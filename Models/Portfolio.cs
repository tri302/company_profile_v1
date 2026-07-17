using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Portfolio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDPortfolio { get; set; }
        public string? UrlGambar { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? Judul { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? IsiContentPortfolio { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? ClientComment { get; set; }
        // Kolom Multi-Bahasa
        public string? Judul_ID { get; set; }
        public string? Judul_EN { get; set; }
        public string? IsiContentPortfolio_ID { get; set; }
        public string? IsiContentPortfolio_EN { get; set; }
        public string ClientComment_ID { get; set; } = null!;
        public string ClientComment_EN { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string ClientCompanyName { get; set; } = string.Empty;
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }        

        // Navigation property untuk relasi One-to-Many
        public ICollection<PortfolioGambar> PortfolioGambars { get; set; } = new List<PortfolioGambar>();

        [NotMapped]
        public string? UrlFotoProject { get; set; }
    }
}