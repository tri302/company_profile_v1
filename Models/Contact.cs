using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDContact { get; set; }

        //menampung filter multi-bahasa
        [NotMapped]
        [ValidateNever]
        public string? Alamat { get; set; }
        // Kolom Multi-Bahasa
        public string? Alamat_ID { get; set; }
        public string? Alamat_EN { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

        public string UrlFb { get; set; } = string.Empty;
        public string UrlTwiter { get; set; } = string.Empty;
        public string UrlLinkedIn { get; set; } = string.Empty;
        public string UrlPinterest { get; set; } = string.Empty;
        public string UrlYoutube { get; set; } = string.Empty;
        public string UrlInstagram { get; set; } = string.Empty;
        //public string UrlTiktok { get; set; } = string.Empty;
        //public string UrlGooglePlus { get; set; } = string.Empty;

        public DateTime CreateAt { get; set; }
    }
}