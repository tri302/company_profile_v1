

namespace company_profile
{
    public interface IKategoriRepo
    {
        //Get All Kategori
        Task<IEnumerable<KategoriVM>> GetAllKategoriAsync();

        //Update Kategori
        Task<bool> UpdateKategoriAsync(KategoriVM kategoriVM);

        //Create Kategori    
        Task<KategoriVM> CreateKategoriAsync(KategoriVM kategoriVM);

        //Delete Kategori
        Task<bool> DeleteKategoriAsync(int idKategori);

        //Get Kategori by Id
        Task<KategoriVM> GetKategoriByIdAsync(int idKategori);
    }
}

