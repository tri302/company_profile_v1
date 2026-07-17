namespace company_profile
{
    public interface INewsRepo
    {
        //Get All News
        Task<IEnumerable<NewsVM>> GetAllNewIndexAsync();       
        NewsVM GetAllNews();

        //Update News
        Task<bool> UpdateNewsAsync(NewsVM newsVM);

        //Create News    
        Task<NewsVM> CreateNewsAsync(NewsVM newsVM);

        //Delete News
        Task<bool> DeleteNewsAsync(int idNews);

        //Get News by Id
        Task<NewsVM> GetNewsByIdAsync(int idNews);

        Task<NewsVM> GetDetailNewsByIdAsync(int idNews);

        Task<NewsVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();

        NewsVM GetPagedDataFE(int pageIndex, int pageSize);
        Task<NewsVM> GetPagedDataFENewsByCategory(string namaKategori, int pageIndex, int pageSize);
    }
}

