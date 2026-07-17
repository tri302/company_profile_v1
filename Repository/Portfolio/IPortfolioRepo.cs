namespace company_profile
{
    public interface IPortfolioRepo
    {
        //Get All Portfolio
        Task<IEnumerable<PortfolioVM>> GetAllPortfolioAsync();

        //Update Portfolio
        Task<bool> UpdatePortfolioAsync(PortfolioVMEdit portfolioVM);

        //Create Portfolio    
        Task<PortfolioVM> CreatePortfolioAsync(PortfolioVM portfolioVM);

        //Delete Portfolio
        Task<bool> DeletePortfolioAsync(int idPortfolio);

        //Get Portfolio by Id
        Task<PortfolioVM> GetPortfolioByIdAsync(int idPortfolio);

        Task<PortfolioVMEdit> GetPortfolioForEditAsync(int idPortfolio);

        Task<PortfolioVM> GetPagedDataAsync(int pageIndex, int pageSize);        
        Task<int> CountAsync();
        Task<IEnumerable<PortfolioVM>> GetAllPortfolioAsyncTake();
    }
}

