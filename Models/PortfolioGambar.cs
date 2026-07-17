using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    [Table("portfoliogambar")]
    public class PortfolioGambar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UrlGambar { get; set; }

        [NotMapped]
        public string Judul { get; set; } = string.Empty;

        // Foreign key untuk relasi dengan Portfolio
        public int IDPortfolio { get; set; }
        [ForeignKey("IDPortfolio")]
        public virtual Portfolio? Portfolio { get; set; }

    }
}