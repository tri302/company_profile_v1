namespace company_profile
{
    public interface IEmailService
    {
        // Untuk kirim email satu per satu (Contact Us, Welcome Email, dll)
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);

        // Untuk kirim email massal sekaligus (Digunakan oleh Quartz)
        Task SendBulkEmailAsync(List<string> toEmails, string subject, string htmlMessage);
        
        // Tambahkan method ini agar bisa menerima konfigurasi dinamis
        Task SendEmailWithConfigAsync(string toEmail, string subject, string htmlContent, EmailSetting config);
    }
}