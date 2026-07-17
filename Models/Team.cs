using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDTeam { get; set; }

        //menampung filter multi bahasa
        [NotMapped]
        public string? Jabatan { get; set; }
        //menampung filter multi bahasa
        [NotMapped]
        public string? Deskripsi { get; set; }

        public string? UrlFoto { get; set; }
        public string? Email { get; set; }
        public string? NoHP { get; set; }
        public string? Website { get; set; }
        public string? Nama { get; set; }
        public string? UrlFb { get; set; }
        public string? UrlLinkedIn { get; set; }
        public string? UrlYoutube { get; set; }
        public string? UrlTwiter { get; set; }

        // Kolom Multi-Bahasa
        public string? Jabatan_ID { get; set; }
        public string? Jabatan_EN { get; set; }
        public string? Deskripsi_ID { get; set; }
        public string? Deskripsi_EN { get; set; }
    }
}