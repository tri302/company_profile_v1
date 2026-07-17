using company_profile;
using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace company_profile
{
    [DisallowConcurrentExecution]
    public class BroadcastJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public BroadcastJob(IServiceProvider serviceProvider, IWebHostEnvironment env, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _env = env;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"[QUARTZ] Memulai pengecekan newsletter pada: {DateTime.Now}");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                // ==========================================
                // AMBIL SETTING EMAIL DARI DATABASE
                // ==========================================
                var emailConfig = await dbContext.EmailSettings.FirstOrDefaultAsync(); // Sesuaikan nama DbSet Anda
                if (emailConfig == null || string.IsNullOrEmpty(emailConfig.SmtpHost))
                {
                    Console.WriteLine("[QUARTZ] Gagal! Konfigurasi SMTP email di database tidak ditemukan.");
                    return;
                }

                // 1. Ambil berita yang belum selesai dikirim (Tambahkan .AsNoTracking())
                var pendingNews = await dbContext.News
                    .Include(n => n.Kategori)
                    .AsNoTracking() // 🔥 KUNCINYA DI SINI! Putus tracking agar EF Core tidak overthinking
                    .FirstOrDefaultAsync(n => !n.IsSent);

                if (pendingNews == null)
                {
                    Console.WriteLine("[QUARTZ] Selesai. Tidak ada berita baru dengan status IsSent = 0.");
                    return;
                }

                int totalSubscribers = await dbContext.Subscribers.CountAsync();
                var currentStrategy = StrategyDeterminer.GetStrategy(totalSubscribers);

                // 2. Ambil subscriber yang BELUM pernah menerima berita ini
                var subscribers = await dbContext.Subscribers
                    .Where(s => !dbContext.NewsLog.Any(log => log.IDNews == pendingNews.IDNews && log.Email == s.Email))
                    .Select(s => s.Email)
                    .Take(currentStrategy.BatchSize)
                    .ToListAsync();

                // ==========================================
                // SCENARIO A: Jika di awal batch ternyata subscriber-nya sudah 0
                // ==========================================
                if (subscribers.Count == 0)
                {
                    Console.WriteLine($"[QUARTZ] Semua subscriber sudah menerima berita '{pendingNews.Judul}'. Menandai IsSent = true.");

                    // 🔥 Beri tahu EF Core kalau entity ini mau diupdate meskipun AsNoTracking
                    pendingNews.IsSent = true;
                    dbContext.Entry(pendingNews).State = EntityState.Modified;

                    await dbContext.SaveChangesAsync();
                    return;
                }

                // 3. Ambil template HTML
                string templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "NewsNewsletter.html");
                if (!File.Exists(templatePath))
                {
                    Console.WriteLine($"[QUARTZ] File template tidak ada di: {templatePath}");
                    return;
                }

                string emailBodyTemplate = await File.ReadAllTextAsync(templatePath);

                // 4. Olah teks & placeholder HTML
                string cleanText = System.Text.RegularExpressions.Regex.Replace(pendingNews.IsiContent ?? "", "<.*?>", string.Empty);
                string shortText = cleanText.Length > 180 ? cleanText.Substring(0, 180) + "..." : cleanText;

                string baseUrl = _configuration["AppConfig:BaseUrl"] ?? "http://localhost:5171";
                string linkDetail = $"{baseUrl}/Company/NewsDetail/{pendingNews.IDNews}";

                string urlGambarFinal = string.IsNullOrEmpty(pendingNews.UrlGambar)
                    ? $"{baseUrl}/uploads/no-image.png"
                    : (pendingNews.UrlGambar.StartsWith("http") ? pendingNews.UrlGambar : $"{baseUrl}{pendingNews.UrlGambar}");

                string finalEmailBody = emailBodyTemplate
                    .Replace("{Judul}", pendingNews.Judul)
                    .Replace("{UrlGambar}", urlGambarFinal)
                    .Replace("{Kategori}", pendingNews.Kategori?.NamaKategori ?? "General")
                    .Replace("{Tanggal}", pendingNews.CreateAt.ToString("MMMM dd, yyyy"))
                    .Replace("{ShortContent}", shortText)
                    .Replace("{LinkDetail}", linkDetail);

                Console.WriteLine($"[QUARTZ] Mengirim email broadcast untuk berita ID {pendingNews.IDNews} ke {subscribers.Count} penerima.");

                // Gunakan list untuk menampung email yang benar-benar sukses terkirim
                var successfulEmails = new List<string>();

                // 💡 Iterasi per email dengan menyertakan emailConfig dari DB
                foreach (var email in subscribers)
                {
                    try
                    {
                        // 🔥 PANGGIL METHOD BARU DAN SERTAKAN EMAILCONFIG DARI DB
                        await emailService.SendEmailWithConfigAsync(email, pendingNews.Judul, finalEmailBody, emailConfig);
                        successfulEmails.Add(email);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[QUARTZ] Gagal mengirim email ke {email}. Error: {ex.Message}");
                    }
                }

                // 5. Jika ada email yang sukses terkirim, catat lognya
                if (successfulEmails.Any())
                {
                    var logs = successfulEmails.Select(email => new NewsLog
                    {
                        IDNews = pendingNews.IDNews,
                        Email = email,
                        SentAt = DateTime.Now
                    });

                    await dbContext.NewsLog.AddRangeAsync(logs);
                    await dbContext.SaveChangesAsync();
                }

                // ==========================================
                // SCENARIO B: Pengecekan setelah seluruh email di batch ini sukses terkirim
                // ==========================================
                int totalSentForThisNews = await dbContext.NewsLog.CountAsync(log => log.IDNews == pendingNews.IDNews);

                if (subscribers.Count < currentStrategy.BatchSize || totalSentForThisNews >= totalSubscribers)
                {
                    // 🔥 Beri tahu EF Core kalau entity ini mau diupdate meskipun AsNoTracking
                    pendingNews.IsSent = true;
                    dbContext.Entry(pendingNews).State = EntityState.Modified;

                    await dbContext.SaveChangesAsync();

                    string successTime = DateTime.Now.ToString("HH:mm:ss");
                    Console.WriteLine($"[{successTime}] [DATABASE UPDATE] Seluruh batch selesai. Sukses mengubah IsSent = 1 untuk ID News: {pendingNews.IDNews}");
                }
                else
                {
                    Console.WriteLine($"[QUARTZ] Batch berhasil diproses. Tersisa {totalSubscribers - totalSentForThisNews} user untuk batch berikutnya.");
                }
            }
        }
    }
}