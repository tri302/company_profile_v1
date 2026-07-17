using company_profile;

namespace company_profile
{
    public interface IIndexHome
    {
        Task<IndexHomeVM> GetAllAsync();
    }
}