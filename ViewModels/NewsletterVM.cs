using System.ComponentModel.DataAnnotations;

namespace company_profile
{
    public class NewsletterVM
    {
        [Required(ErrorMessage = "Email tidak boleh kosong.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [Display(Prompt = "Email Address")]
        public string Email { get; set; } = string.Empty;
    
    }
}