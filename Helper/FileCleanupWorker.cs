using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;

namespace company_profile.Helper
{
    public class FileCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public FileCleanupWorker(IServiceProvider serviceProvider, IConfiguration config, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _env = env;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Jalankan pembersihan setiap 24 jam
                try
                {
                    CleanupOrphanedFiles();
                }
                catch (Exception ex)
                {
                    // Logging error
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                // Ubah ini untuk testing
                //await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private void CleanupOrphanedFiles()
        {
            string folderRelativePath = _config["FileSettings:ImageUploadPath"] ?? "uploads/ckeditor/images";
            string targetFolder = Path.Combine(_env.ContentRootPath, folderRelativePath);

            if (!Directory.Exists(targetFolder)) return;

            // 1. Ambil semua file fisik yang ada di folder
            var allFiles = Directory.GetFiles(targetFolder);

            // 2. Ambil referensi gambar yang digunakan dari database
            var usedFiles = new HashSet<string>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var newsList = dbContext.News.ToList();

                foreach (var news in newsList)
                {
                    // Tambahkan file dari konten ID
                    AddUsedFilesFromHtml(news.IsiContent_ID, usedFiles);
                    // Tambahkan file dari konten EN
                    AddUsedFilesFromHtml(news.IsiContent_EN, usedFiles);
                }
            }


            // 3. Loop file fisik dan hapus jika tidak digunakan            
            foreach (var filePath in allFiles)
            {
                string fileName = Path.GetFileName(filePath);

                // Cek jika file TIDAK ada di daftar yang digunakan
                if (!usedFiles.Contains(fileName))
                {
                    var fileInfo = new FileInfo(filePath);

                    // Hapus hanya jika file sudah berumur > 1 jam 
                    if (fileInfo.CreationTime < DateTime.Now.AddHours(-1))
                    {
                        try
                        {
                            Console.WriteLine($"DEBUG: File {fileName} tidak digunakan, akan dihapus...");
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Gagal hapus file {fileName}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: File {fileName} tidak digunakan, tapi masih baru (Grace Period). Lewati.");
                    }
                }
                else
                {
                    // Opsional: Untuk memastikan log tidak terlalu panjang, 
                    // Anda bisa menaruh ini di komentar saja jika file banyak.
                     Console.WriteLine($"DEBUG: File {fileName} masih digunakan.");
                }
            }
        }

        // Helper method untuk mengekstrak dan membersihkan nama file
        private void AddUsedFilesFromHtml(string html, HashSet<string> usedFiles)
        {
            var urls = ExtractImageUrls(html ?? "");
            foreach (var url in urls)
            {
                // Bersihkan URL untuk mendapatkan nama file murni
                // Contoh: /News/GetImage?fileName=123.png -> 123.png
                var uri = new Uri("http://dummy" + url); // Tambahkan dummy host agar bisa di-parse
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string fileName = query["fileName"];

                // Jika tidak ada parameter fileName, ambil dari path terakhir
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.GetFileName(uri.AbsolutePath);
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    usedFiles.Add(fileName);
                }
            }
        }


        private List<string> ExtractImageUrls(string html)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(html)) return list;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var imgNodes = doc.DocumentNode.SelectNodes("//img");
            if (imgNodes != null)
            {
                foreach (var node in imgNodes)
                {
                    var src = node.GetAttributeValue("src", "");
                    if (!string.IsNullOrEmpty(src)) list.Add(src);
                }
            }
            return list;
        }
    }
}
