using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace company_profile
{
    public class UserContactedRepo : IUserContactedRepo
    {
        private readonly AppDbContext _context;

        public UserContactedRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ContactFormVM> CreateContactAsync(ContactFormVM contactVM)
        {          
            var model = new ContactForm
            {
                Id = contactVM.Id,
                Name = contactVM.Name,                
                PhoneNumber = contactVM.PhoneNumber,
                EmailAddress = contactVM.EmailAddress,
                Pesan = contactVM.Pesan,
                Subject = contactVM.Subject
            };
            _context.ContactForms.Add(model);
            await _context.SaveChangesAsync();
            return contactVM;
        }

        public async Task<bool> DeleteContactAsync(int idContact)
        {
            var contact = await _context.ContactForms.FindAsync(idContact);
            if (contact == null) return false;

            _context.ContactForms.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ContactFormVM>> GetAllContactAsync()
        {
            return await GetQuery().ToListAsync();
        }       

        public async Task<ContactFormVM> GetContactByIdAsync(int idContact)
        {
            var contact = await _context.ContactForms.FindAsync(idContact);
            if (contact == null) return null;
            return new ContactFormVM
            {
                Id = contact.Id,
                Name = contact.Name,                
                PhoneNumber = contact.PhoneNumber,
                EmailAddress = contact.EmailAddress,
                Subject = contact.Subject,
                Pesan = contact.Pesan
            };
        }
        public async Task<ContactFormVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ContactFormVM
            {
                List = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.ContactForms.CountAsync();
        }

        private IQueryable<ContactFormVM> GetQuery()
        {
            var contacts = _context.ContactForms;
            return contacts.Select(c => new ContactFormVM
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,
                Pesan = c.Pesan,
                Subject = c.Subject
            });
        }
    }
}

