using System.Collections.Generic;
using System.Threading.Tasks;

namespace company_profile
{
    public interface ISliderFooterRepo
    {
        Task<IEnumerable<SliderFooterHomeVM>> GetAllAsync();
        Task<SliderFooterHomeVM?> GetByIdAsync(int id);
        Task<bool> InsertAsync(SliderFooterHomeVM model);
        Task<bool> UpdateAsync(SliderFooterHomeVM model);
        Task<bool> DeleteAsync(int id);
        Task<bool> SaveAsync();

        Task<IEnumerable<SliderFooterHomeVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}