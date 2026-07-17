using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CompanyProfile.Helper;

namespace company_profile
{
    public class EventsRepo : IEventsRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public EventsRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<EventsVM> CreateEventsAsync(EventsVM eventsVM)
        {
            var model = new Events
            {
                // Pemetaan Properti Bahasa Indonesia
                JudulEvent_ID = eventsVM.JudulEvent_ID,
                AlamatEvent_ID = eventsVM.AlamatEvent_ID,
                IsiContentEvent_ID = eventsVM.IsiContentEvent_ID,
                LokasiEvent_ID = StringHelper.ExtractIframeSrc(eventsVM.LokasiEvent_ID),

                // Pemetaan Properti Bahasa Inggris
                JudulEvent_EN = eventsVM.JudulEvent_EN,
                AlamatEvent_EN = eventsVM.AlamatEvent_EN,
                IsiContentEvent_EN = eventsVM.IsiContentEvent_EN,
                LokasiEvent_EN = StringHelper.ExtractIframeSrc(eventsVM.LokasiEvent_EN),

                // Properti Universal
                StartEvent = eventsVM.StartEvent,
                EndEvent = eventsVM.EndEvent
            };

            if (eventsVM.UploadGambar != null && eventsVM.UploadGambar.Length > 0)
            {
                model.UrlGambar = await SaveImage(eventsVM.UploadGambar);
            }
            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return new EventsVM
            {
                IDEvents = model.IDEvents,
                UrlGambar = model.UrlGambar,
                StartEvent = model.StartEvent,
                EndEvent = model.EndEvent,

                // Kembalikan data ter-update ke VM (Termasuk URL Map yang sudah di-extract)
                JudulEvent_ID = model.JudulEvent_ID,
                AlamatEvent_ID = model.AlamatEvent_ID,
                IsiContentEvent_ID = model.IsiContentEvent_ID,
                LokasiEvent_ID = model.LokasiEvent_ID,

                JudulEvent_EN = model.JudulEvent_EN,
                AlamatEvent_EN = model.AlamatEvent_EN,
                IsiContentEvent_EN = model.IsiContentEvent_EN,
                LokasiEvent_EN = model.LokasiEvent_EN,

                // Cadangan untuk compatibility halaman Index lama (jika ada)
                JudulEvent = model.JudulEvent_ID,
                AlamatEvent = model.AlamatEvent_ID
            };
        }

        public async Task<bool> UpdateEventsAsync(EventsVM eventsVM)
        {
            var model = await _context.Events.FindAsync(eventsVM.IDEvents);
            if (model == null) return false;

            // Update Data Bahasa Indonesia
            model.JudulEvent_ID = eventsVM.JudulEvent_ID;
            model.AlamatEvent_ID = eventsVM.AlamatEvent_ID;
            model.IsiContentEvent_ID = eventsVM.IsiContentEvent_ID;
            model.LokasiEvent_ID = StringHelper.ExtractIframeSrc(eventsVM.LokasiEvent_ID);

            // Update Data Bahasa Inggris
            model.JudulEvent_EN = eventsVM.JudulEvent_EN;
            model.AlamatEvent_EN = eventsVM.AlamatEvent_EN;
            model.IsiContentEvent_EN = eventsVM.IsiContentEvent_EN;
            model.LokasiEvent_EN = StringHelper.ExtractIframeSrc(eventsVM.LokasiEvent_EN);

            // Update Data Universal
            model.StartEvent = eventsVM.StartEvent;
            model.EndEvent = eventsVM.EndEvent;

            if (eventsVM.UploadGambar != null && eventsVM.UploadGambar.Length > 0)
            {
                // Hapus foto lama jika ada
                DeleteOldImage(model.UrlGambar);

                // Simpan foto baru
                model.UrlGambar = await SaveImage(eventsVM.UploadGambar);
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventsAsync(int idEvents)
        {
            var events = await _context.Events.FindAsync(idEvents);
            if (events == null) return false;
            DeleteOldImage(events.UrlGambar);
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventsVM>> GetAllEventsAsync()
        {
            return await GetQuery().ToListAsync();
        }

        public async Task<EventsVM> GetEventsByIdAsync(int idEvents)
        {
            var events = await _context.Events.FindAsync(idEvents);
            string currentLang = CultureInfo.CurrentCulture.Name;
            if (events == null) return null;

            var today = DateTime.Now;

            var upcomingEvents = await _context.Events
                .Where(x => x.StartEvent > today && x.IDEvents != idEvents)
                .OrderBy(x => x.StartEvent)
                .Take(5)
                .ToListAsync();

            var pastEvents = await _context.Events
                .Where(x => x.EndEvent < today && x.IDEvents != idEvents)
                .OrderByDescending(x => x.EndEvent)
                .Take(5)
                .ToListAsync();

            return new EventsVM
            {
                IDEvents = events.IDEvents,
                StartEvent = events.StartEvent,
                EndEvent = events.EndEvent,
                UrlGambar = events.UrlGambar,

                JudulEvent = currentLang.StartsWith("id") ? events.JudulEvent_ID : events.JudulEvent_EN,
                AlamatEvent = currentLang.StartsWith("id") ? events.AlamatEvent_ID : events.AlamatEvent_EN,
                IsiContentEvent = currentLang.StartsWith("id") ? events.IsiContentEvent_ID : events.IsiContentEvent_EN,
                LokasiEvent = currentLang.StartsWith("id") ? events.LokasiEvent_ID : events.LokasiEvent_EN,

                // Pemetaan Data Utama (ID)
                JudulEvent_ID = events.JudulEvent_ID,
                AlamatEvent_ID = events.AlamatEvent_ID,
                IsiContentEvent_ID = events.IsiContentEvent_ID,
                LokasiEvent_ID = events.LokasiEvent_ID,

                // Pemetaan Data Utama (EN)
                JudulEvent_EN = events.JudulEvent_EN,
                AlamatEvent_EN = events.AlamatEvent_EN,
                IsiContentEvent_EN = events.IsiContentEvent_EN,
                LokasiEvent_EN = events.LokasiEvent_EN,
                UpcomingEvents = upcomingEvents,
                PastEvents = pastEvents
            };
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            string wwwrootPath = _webHost.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string folderPath = Path.Combine(wwwrootPath, "uploads", "events");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/events/" + fileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            try
            {
                string rootPath = _webHost.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                string fullPath = Path.Combine(rootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Gagal hapus gambar lama: {ex.Message}");
            }
        }

        public async Task<IEnumerable<EventsVM>> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            return await GetQuery()
                .OrderByDescending(x => x.IDEvents)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }        

        public async Task<int> CountAsync()
        {
            return await _context.Events.CountAsync();
        }

        private IQueryable<EventsVM> GetQuery()
        {
            string currentLang = CultureInfo.CurrentCulture.Name;
            return _context.Events.Select(e => new EventsVM
            {
                IDEvents = e.IDEvents,
                StartEvent = e.StartEvent,
                EndEvent = e.EndEvent,
                UrlGambar = e.UrlGambar,

                JudulEvent = currentLang.StartsWith("id") ? e.JudulEvent_ID : e.JudulEvent_EN,
                AlamatEvent = currentLang.StartsWith("id") ? e.AlamatEvent_ID : e.AlamatEvent_EN,
                IsiContentEvent = currentLang.StartsWith("id") ? e.IsiContentEvent_ID : e.IsiContentEvent_EN,
                LokasiEvent = currentLang.StartsWith("id") ? e.LokasiEvent_ID : e.LokasiEvent_EN,

                // Pemetaan Multi-Bahasa ke ViewModel List
                JudulEvent_ID = e.JudulEvent_ID,
                AlamatEvent_ID = e.AlamatEvent_ID,
                IsiContentEvent_ID = e.IsiContentEvent_ID,
                LokasiEvent_ID = e.LokasiEvent_ID,

                JudulEvent_EN = e.JudulEvent_EN,
                AlamatEvent_EN = e.AlamatEvent_EN,
                IsiContentEvent_EN = e.IsiContentEvent_EN,
                LokasiEvent_EN = e.LokasiEvent_EN
            });
        }
    }
}