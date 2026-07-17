using System.ComponentModel.DataAnnotations;

namespace company_profile.ViewModels
{
    public class HeaderVM
    {
        public int ID { get; set; }
        
        [Display(Name = "Url Facebook")]
        public string? UrlFb { get; set; }
        
        [Display(Name = "Url Twitter")]
        public string? UrlTwiter { get; set; }
        
        [Display(Name = "Url LinkedIn")]
        public string? UrlLinkedIn { get; set; }

        [Display(Name = "Url Pinterest")]
        public string? UrlPinterest { get; set; }

        [Display(Name = "Url Youtube")]
        [DataType(DataType.Url)]
        public string? UrlYoutube { get; set; }

        [Display(Name = "Url Instagram")]        
        public string? UrlInstagram { get; set; }

        [Display(Name = "Url Tiktok")]        
        public string? UrlTiktok { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone Number is required.")]
        public string PhoneNumber { get; set; } = string.Empty;                
        public bool IsShow { get; set; } = false;
    }
}