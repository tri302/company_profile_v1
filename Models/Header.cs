using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile.Models
{
    public class Header
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string UrlFb { get; set; } = string.Empty;
        public string UrlTwiter { get; set; } = string.Empty;
        public string UrlLinkedIn { get; set; } = string.Empty;
        public string UrlPinterest { get; set; } = string.Empty;
        public string UrlYoutube { get; set; } = string.Empty;
        public string UrlInstagram { get; set; } = string.Empty;
        public string UrlTiktok { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsShow { get; set; } = false;
    }
}