using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace company_profile
{
    public class ContactFormVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "No HP wajib diisi")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pesan wajib diisi")]
        public string Pesan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject wajib diisi")]
        public string Subject { get; set; } = string.Empty;

        [ValidateNever]
        public int TotalPages { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public List<ContactFormVM> List { get; set; } = new List<ContactFormVM>();

    }
}