using AngleSharp.Io;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace company_profile
{
    public interface IImageCleanupService
    {
        void DeleteRemovedImages(string oldHtml, string newHtml);
        void DeleteAllImagesInContent(string html);
    }

    public class ImageCleanupService : IImageCleanupService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public ImageCleanupService(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void DeleteAllImagesInContent(string html)
        {
            if (string.IsNullOrEmpty(html)) return;
            var images = ExtractImageUrlsFromContents(html);
            // Mengambil path dari appsettings.json
            string folderRelativePath = _config["FileSettings:ImageUploadPath"] ?? "uploads/ckeditor/images";
            // Menggunakan ContentRootPath agar dinamis (bukan wwwroot)
            string baseFolder = Path.Combine(_env.ContentRootPath, folderRelativePath);
            foreach (var imgUrl in images)
            {
                var uri = new Uri("http://dummy" + imgUrl);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

                string fileName = query["fileName"];
                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(uri.AbsolutePath);
                string filePath = Path.Combine(baseFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    Console.WriteLine("DEBUG: File TIDAK ditemukan!");
                }
            }
        }

        public List<string> ExtractImageUrlsFromContents(string html)
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

        public void DeleteRemovedImages(string oldHtml, string newHtml)
        {
            // Gunakan path dari config agar konsisten
            string folderRelativePath = _config["FileSettings:ImageUploadPath"] ?? "uploads/ckeditor/images";
            string baseFolder = Path.Combine(_env.ContentRootPath, folderRelativePath);

            var oldImages = ExtractImageUrlsFromContents(oldHtml ?? "");
            var newImages = ExtractImageUrlsFromContents(newHtml ?? "");

            foreach (var imgUrl in oldImages)
            {
                if (!newImages.Contains(imgUrl))
                {
                    // Ambil nama file dari URL
                    string fileName = Path.GetFileName(imgUrl);

                    // Hapus query string jika ada (misal: image.jpg?v=123)
                    if (fileName.Contains("?"))
                        fileName = fileName.Substring(0, fileName.IndexOf("?"));

                    string filePath = Path.Combine(baseFolder, fileName);

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            // Log error jika file sedang digunakan atau akses ditolak
                           Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }        
    }
}