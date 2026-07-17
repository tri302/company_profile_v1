using System.Globalization;
namespace company_profile
{
    public class IndexHome : IIndexHome
    {
        private readonly IAchievementsRepo _achievementsRepo;
        private readonly IBannerHomeRepo _bannerHomeRepo;
        private readonly IHeaderRepo _headerRepo;
        private readonly IHomeRepo _homeRepo;
        private readonly IWhyChooseUsRepo _whyChooseUsRepo;
        private readonly ISpecialFeaturesRepo _specialFeaturesRepo;
        private readonly IOurServicesRepo _ourServicesRepo;
        private readonly IPortfolioRepo _portfolioRepo;
        private readonly ITeamRepo _teamRepo;
        private readonly IPricingRepo _pricingRepo;
        private readonly IFaqRepo _faq;
        private readonly INewsRepo _newsRepo;

        public IndexHome(
        IAchievementsRepo achievementsRepo,
        IBannerHomeRepo bannerHomeRepo,
        IHeaderRepo headerRepo,
        IHomeRepo homeRepo,
        IWhyChooseUsRepo whyChooseUsRepo,
        ISpecialFeaturesRepo specialFeaturesRepo,
        IOurServicesRepo ourServicesRepo,
        IPortfolioRepo portfolioRepo,
        ITeamRepo teamRepo,
        IPricingRepo pricingRepo,
        IFaqRepo faqRepo,
        INewsRepo newsRepo
         )
        {
            _achievementsRepo = achievementsRepo;
            _bannerHomeRepo = bannerHomeRepo;
            _headerRepo = headerRepo;
            _homeRepo = homeRepo;
            _whyChooseUsRepo = whyChooseUsRepo;
            _specialFeaturesRepo = specialFeaturesRepo;
            _ourServicesRepo = ourServicesRepo;
            _portfolioRepo = portfolioRepo;
            _teamRepo = teamRepo;
            _pricingRepo = pricingRepo;
            _faq = faqRepo;
            _newsRepo = newsRepo;
        }
        public async Task<IndexHomeVM> GetAllAsync()
        {
            // Kita panggil satu per satu secara berurutan menggunakan await.
            // Ini menjamin DbContext menyelesaikan satu kueri sebelum kueri berikutnya dimulai.
            var indexHomeVM = new IndexHomeVM
            {
                Achievements = await _achievementsRepo.GetAllAsync(),
                BannerHome = await _bannerHomeRepo.GetAllAsync(),
                Header = await _headerRepo.GetAllAsync(),
                Home = await _homeRepo.GetAllAsync(),
                WhyChooseUs = await _whyChooseUsRepo.GetAllAsync(),
                SpecialFeatures = await _specialFeaturesRepo.GetAllSpecialFeaturesAsync(),
                OurServices = await _ourServicesRepo.GetAllOurServicesAsyncTake(),
                Portfolio = await _portfolioRepo.GetAllPortfolioAsyncTake(),
                Team = await _teamRepo.GetAllTeamAsync(),
                Pricing = await _pricingRepo.GetAllPricingAsyncTake(),
                Faq = await _faq.GetAllFaqAsyncTake(),
                News = await _newsRepo.GetAllNewIndexAsync(),
                SliderFooterHome = await _bannerHomeRepo.GetAllSliderFooterHomeAsync()
            };
            return indexHomeVM;
        }
    }
}