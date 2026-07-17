

namespace company_profile
{
    public interface IContactRepo
    {
        Task<ContactVM> GetFirstContactAsync();

        //Get All Contact
        Task<IEnumerable<ContactVM>> GetAllContactAsync();

        //Update Contact
        Task<bool> UpdateContactAsync(ContactVM contactVM);

        //Create Contact    
        Task<ContactVM> CreateContactAsync(ContactVM contactVM);

        //Delete Contact
        Task<bool> DeleteContactAsync(int idContact);

        //Get Contact by Id
        Task<ContactVM> GetContactByIdAsync(int idContact);

        Task<ContactVM> GetAllContactIndexAsync();
    }
}

