using System.Collections.Generic;
using System.Threading.Tasks;
using company_profile;
using company_profile.ViewModels.HomeVM;
using Microsoft.AspNetCore.Http;

public interface IHomeRepo
{
    Task<IEnumerable<HomeVM>> GetAllAsync();
    Task<HomeVM?> GetByIdAsync(int id);
    Task<bool> InsertAsync(HomeVM model);
    Task<bool> UpdateAsync(HomeVM model);
    Task<bool> DeleteAsync(int id);
    Task<HomeVM?> DetailsAsync(int id);

    Task<HomeVM> GetPagedDataAsync(int pageIndex, int pageSize);
    Task<int> CountAsync();
}