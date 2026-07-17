using System.ComponentModel.DataAnnotations;

namespace company_profile.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Kata sandi saat ini wajib diisi.")]
        [DataType(DataType.Password)]
        [Display(Name = "Kata Sandi Saat Ini")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Kata sandi baru wajib diisi.")]
        [StringLength(100, ErrorMessage = "Kata sandi minimal {2} dan maksimal {1} karakter.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Kata Sandi Baru")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Kata Sandi Baru")]
        [Compare("NewPassword", ErrorMessage = "Kata sandi baru dan konfirmasi tidak cocok.")]
        public string ConfirmPassword { get; set; }
    }
}
