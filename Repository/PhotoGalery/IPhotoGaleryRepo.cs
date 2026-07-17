

namespace company_profile
{
    public interface IPhotoGaleryRepo
    {
        //Get All PhotoGalery
        Task<IEnumerable<PhotoGalleryVM>> GetAllPhotoGaleryAsync();

        //Update PhotoGalery
        Task<bool> UpdatePhotoGaleryAsync(PhotoGalleryVM photoGaleryVM);

        //Create PhotoGalery    
        Task<PhotoGalleryVM> CreatePhotoGaleryAsync(PhotoGalleryVM photoGaleryVM);

        //Delete PhotoGalery
        Task<bool> DeletePhotoGaleryAsync(int idPhotoGalery);

        //Get PhotoGalery by Id
        Task<PhotoGalleryVM> GetPhotoGaleryByIdAsync(int idPhotoGalery);

        Task<PhotoGalleryVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}

