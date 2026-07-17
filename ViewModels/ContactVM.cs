using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile
{
    public class ContactVM
    {
        public int IDContact { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        [ValidateNever]
        public string? Alamat { get; set; }

        [Required(ErrorMessage = "Alamat tidak boleh kosong !")]
        [Display(Name = "Alamat dalam Bahasa Indonesia")]
        // Kolom Multi-Bahasa
        public string? Alamat_ID { get; set; }

        [Required(ErrorMessage = "Wajib di isi !")]
        [Display(Name = "Alamat dalam Bahasa Inggris")]
        public string? Alamat_EN { get; set; }

        [Required(ErrorMessage = "Phone Number tidak boleh kosong !")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email Address tidak boleh kosong !")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Email Address tidak valid !")]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? UrlFb { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? UrlTwiter { get; set; }

        [NotMapped]
        [ValidateNever]
        public string?   UrlLinkedIn { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? UrlPinterest { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? UrlYoutube { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? UrlInstagram { get; set; }
        //public string? UrlTiktok { get; set; } 
        //public string? UrlGooglePlus { get; set; } 

        [ValidateNever]
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        [NotMapped]
        public List<ContactVM>? ListContact { get; set; }

        [ValidateNever]
        [NotMapped]
        public ContactFormVM ContactForm { get; set; } = new ContactFormVM();

    }
}