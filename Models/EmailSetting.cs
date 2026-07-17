using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class EmailSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email pengirim wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string SendEmailFrom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email penerima wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string ReceiveEmailTo { get; set; } = string.Empty;

        [Required(ErrorMessage = "SMTP Host wajib diisi")]
        public string SmtpHost { get; set; } = string.Empty;

        [Required(ErrorMessage = "SMTP Port wajib diisi")]
        public int SmtpPort { get; set; }

        [Required(ErrorMessage = "SMTP Username wajib diisi")]
        public string SmtpUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "SMTP Password wajib diisi")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; } = string.Empty;
    }
}