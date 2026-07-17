using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace company_profile
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // 1. KIRIM SATU EMAIL (Koneksi langsung putus)
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = CreateMimeMessage(toEmail, subject, htmlMessage);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        // 2. KIRIM BANYAK EMAIL (Koneksi efisien, buka sekali untuk semua)
        public async Task SendBulkEmailAsync(List<string> toEmails, string subject, string htmlMessage)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);

            foreach (var emailAddress in toEmails)
            {
                try
                {
                    var email = CreateMimeMessage(emailAddress, subject, htmlMessage);
                    await smtp.SendAsync(email);
                    Console.WriteLine($"Berhasil mengirim ke: {emailAddress}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Gagal mengirim ke {emailAddress}: {ex.Message}");
                }
            }

            await smtp.DisconnectAsync(true);
        }

        // Ini method baru untuk Quartz Job
        public async Task SendEmailWithConfigAsync(string toEmail, string subject, string htmlContent, EmailSetting config)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(config.SendEmailFrom, config.SendEmailFrom)); // Nama pengirim bisa disesuaikan
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Hubungkan menggunakan data dari Database
            await client.ConnectAsync(config.SmtpHost, config.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            // Cek jika SMTP memerlukan otentikasi username & password
            if (!string.IsNullOrEmpty(config.SmtpUsername) && !string.IsNullOrEmpty(config.SmtpPassword))
            {
                await client.AuthenticateAsync(config.SmtpUsername, config.SmtpPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // Helper fungsi agar tidak menulis ulang pembuat email
        private MimeMessage CreateMimeMessage(string toEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"], _config["SmtpSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            email.Body = bodyBuilder.ToMessageBody();
            return email;
        }
    }
}