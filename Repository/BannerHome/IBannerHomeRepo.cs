using company_profile.ViewModels;

namespace company_profile 
{
    public interface IBannerHomeRepo
    {
        Task<IEnumerable<BannerHomeVM>> GetAllAsync();        
        Task<IEnumerable<SliderFooterHomeVM>> GetAllSliderFooterHomeAsync();
        Task<BannerHomeVM?> GetByIdAsync(int id);
        Task InsertAsync(BannerHomeVM banner);
        Task UpdateAsync(BannerHomeVM banner);
        Task DeleteAsync(int id);
        Task SaveAsync();    
        // Method untuk menangani validasi dan upload gambar
        Task<string> UploadImageAsync(IFormFile file);
        void DeleteOldImage(string? imageUrl);
    }
}