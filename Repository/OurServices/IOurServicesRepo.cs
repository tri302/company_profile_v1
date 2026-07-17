

namespace company_profile
{
    public interface IOurServicesRepo
    {
        //Get All OurServices
        Task<IEnumerable<OurServicesVM>> GetAllOurServicesAsync();

        //Update OurServices
        Task<bool> UpdateOurServicesAsync(OurServicesVM ourServicesVM);

        //Create OurServices    
        Task<OurServicesVM> CreateOurServicesAsync(OurServicesVM ourServicesVM);

        //Delete OurServices
        Task<bool> DeleteOurServicesAsync(int idOurServices);

        //Get OurServices by Id
        Task<OurServicesVM> GetOurServicesByIdAsync(int idOurServices);

        Task<OurServicesVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
        Task<IEnumerable<OurServicesVM>> GetAllOurServicesAsyncTake();
    }
}

