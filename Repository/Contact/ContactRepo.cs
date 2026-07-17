using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class ContactRepo : IContactRepo
    {
        private readonly AppDbContext _context;

        public ContactRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ContactVM> CreateContactAsync(ContactVM contactVM)
        {
            var model = new Contact
            {
                IDContact = contactVM.IDContact,
                // Pemetaan Kolom Multi-Bahasa
                Alamat_ID = contactVM.Alamat_ID,
                Alamat_EN = contactVM.Alamat_EN,
                PhoneNumber = contactVM.PhoneNumber,
                EmailAddress = contactVM.EmailAddress,

                // Penambahan Pemetaan Media Sosial
                UrlFb = contactVM.UrlFb,
                UrlTwiter = contactVM.UrlTwiter,
                UrlLinkedIn = contactVM.UrlLinkedIn,
                UrlPinterest = contactVM.UrlPinterest,
                UrlYoutube = contactVM.UrlYoutube,
                UrlInstagram = contactVM.UrlInstagram,                
                CreateAt = DateTime.Now
            };

            _context.Contacts.Add(model);
            await _context.SaveChangesAsync();
            return contactVM;
        }

        public async Task<bool> DeleteContactAsync(int idContact)
        {
            var contact = await _context.Contacts.FindAsync(idContact);
            if (contact == null) return false;

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ContactVM>> GetAllContactAsync()
        {
            var contacts = await _context.Contacts.ToListAsync();
            string currentLang = CultureInfo.CurrentCulture.Name;

            return contacts.Select(c => new ContactVM
            {
                IDContact = c.IDContact,
                Alamat = currentLang.StartsWith("id") ? c.Alamat_ID : c.Alamat_EN,
                // Pemetaan Kolom Multi-Bahasa
                Alamat_ID = c.Alamat_ID,
                Alamat_EN = c.Alamat_EN,
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,

                // Pemetaan Social Media
                UrlFb = c.UrlFb,
                UrlTwiter = c.UrlTwiter,
                UrlLinkedIn = c.UrlLinkedIn,
                UrlPinterest = c.UrlPinterest,
                UrlYoutube = c.UrlYoutube,
                UrlInstagram = c.UrlInstagram,
                CreateAt = c.CreateAt
            }).ToList();
        }

        public async Task<ContactVM> GetFirstContactAsync()
        {
            // Ambil hanya satu data dari database
            var c = await _context.Contacts.FirstOrDefaultAsync();

            if (c == null) return null;

            string currentLang = CultureInfo.CurrentCulture.Name;

            return new ContactVM
            {
                IDContact = c.IDContact,
                Alamat = currentLang.StartsWith("id") ? c.Alamat_ID : c.Alamat_EN,                
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,

                // Pemetaan Social Media
                UrlFb = c.UrlFb,
                UrlTwiter = c.UrlTwiter,
                UrlLinkedIn = c.UrlLinkedIn,
                UrlPinterest = c.UrlPinterest,
                UrlYoutube = c.UrlYoutube,
                UrlInstagram = c.UrlInstagram,
                CreateAt = c.CreateAt
            };
        }

        public async Task<ContactVM> GetAllContactIndexAsync()
        {
            // 1. Ambil semua data dari database
            var allContacts = await _context.Contacts.ToListAsync();

            // 2. Cek bahasa saat ini
            string currentLang = CultureInfo.CurrentCulture.Name;

            // 3. Mapping data ke ViewModel
            // Kita memetakan ulang List agar setiap item memiliki data bahasa yang sesuai
            var mappedContacts = allContacts.Select(c => new ContactVM
            {
                IDContact = c.IDContact,
                // Memilih alamat berdasarkan bahasa yang aktif
                Alamat = currentLang.StartsWith("id", StringComparison.OrdinalIgnoreCase) ? c.Alamat_ID : c.Alamat_EN,
                Alamat_ID = c.Alamat_ID,
                Alamat_EN = c.Alamat_EN,
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,

                // Pastikan properti media sosial juga dipetakan di sini
                UrlFb = c.UrlFb,
                UrlTwiter = c.UrlTwiter,
                UrlLinkedIn = c.UrlLinkedIn,
                UrlPinterest = c.UrlPinterest,
                UrlYoutube = c.UrlYoutube,
                UrlInstagram = c.UrlInstagram,              
                CreateAt = c.CreateAt
            }).ToList();

            return new ContactVM
            {
                ListContact = mappedContacts
            };
        }
        public async Task<ContactVM?> GetContactByIdAsync(int idContact)
        {
            var contact = await _context.Contacts.FindAsync(idContact);
            if (contact == null) return null;

            string currentLang = CultureInfo.CurrentCulture.Name;

            return new ContactVM
            {
                IDContact = contact.IDContact,
                Alamat = currentLang.StartsWith("id", StringComparison.OrdinalIgnoreCase) ? contact.Alamat_ID : contact.Alamat_EN,

                // Pemetaan Kolom Multi-Bahasa
                Alamat_ID = contact.Alamat_ID,
                Alamat_EN = contact.Alamat_EN,
                PhoneNumber = contact.PhoneNumber,
                EmailAddress = contact.EmailAddress,

                // Pemetaan Social Media agar muncul saat Edit
                UrlFb = contact.UrlFb,
                UrlTwiter = contact.UrlTwiter,
                UrlLinkedIn = contact.UrlLinkedIn,
                UrlPinterest = contact.UrlPinterest,
                UrlYoutube = contact.UrlYoutube,
                UrlInstagram = contact.UrlInstagram,
                CreateAt = contact.CreateAt
            };
        }

        public async Task<bool> UpdateContactAsync(ContactVM contactVM)
        {
            var contact = await _context.Contacts.FindAsync(contactVM.IDContact);
            if (contact == null) return false;

            // Update entitas database dengan properti multi-bahasa dan kontak
            contact.Alamat_ID = contactVM.Alamat_ID;
            contact.Alamat_EN = contactVM.Alamat_EN;
            contact.PhoneNumber = contactVM.PhoneNumber;
            contact.EmailAddress = contactVM.EmailAddress;

            // Update kolom Social Media
            contact.UrlFb = contactVM.UrlFb;
            contact.UrlTwiter = contactVM.UrlTwiter;
            contact.UrlLinkedIn = contactVM.UrlLinkedIn;
            contact.UrlPinterest = contactVM.UrlPinterest;
            contact.UrlYoutube = contactVM.UrlYoutube;
            contact.UrlInstagram = contactVM.UrlInstagram;
            contact.CreateAt = contactVM.CreateAt;

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}