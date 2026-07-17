

namespace company_profile
{
    public interface IAboutRepo
    {
        //Get All About
        Task<List<AboutVM>> GetAllAboutAsync();
        Task<AboutVM> GetAboutSingleAsync();

        //Update About
        Task<bool> UpdateAboutAsync(AboutVM aboutVM);

        //Create About    
        Task<AboutVM> CreateAboutAsync(AboutVM aboutVM);

        //Delete About
        Task<bool> DeleteAboutAsync(int idAbout);

        //Get About by Id
        Task<AboutVM?> GetAboutByIdAsync(int idAbout);
    }
}

