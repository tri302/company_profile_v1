using System.Collections.Generic;
using System.Threading.Tasks;
using company_profile.ViewModels;

namespace company_profile
{
    public interface IWhyChooseUsRepo
    {
        Task<IEnumerable<WhyChooseUsVM>> GetAllAsync();
        Task<WhyChooseUsVM?> GetByIdAsync(int id);
        Task AddAsync(WhyChooseUsVM whyChooseUs);
        Task UpdateAsync(WhyChooseUsVM whyChooseUs);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();

        //Pagination
        Task<IEnumerable<WhyChooseUsVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}