

namespace company_profile
{
    public interface IUserContactedRepo
    {
        //Get All Contact
        Task<IEnumerable<ContactFormVM>> GetAllContactAsync();        

        //Create Contact    
        Task<ContactFormVM> CreateContactAsync(ContactFormVM contactVM);

        //Delete Contact
        Task<bool> DeleteContactAsync(int idContact);

        //Get Contact by Id
        Task<ContactFormVM> GetContactByIdAsync(int idContact);

        Task<ContactFormVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}

