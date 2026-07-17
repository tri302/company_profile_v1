using company_profile.ViewModels;
using company_profile.ViewModels.HomeVM;

namespace company_profile
{
    public class IndexHomeVM
    {
        public IEnumerable<CounterVM> Achievements { get; set; } = new List<CounterVM>();
        public IEnumerable<BannerHomeVM> BannerHome { get; set; } = new List<BannerHomeVM>();
        public IEnumerable<HeaderVM> Header { get; set; } = new List<HeaderVM>();
        public IEnumerable<HomeVM> Home { get; set; } = new List<HomeVM>();
        public IEnumerable<WhyChooseUsVM> WhyChooseUs { get; set; } = new List<WhyChooseUsVM>();
        public IEnumerable<SpecialFeaturesVM> SpecialFeatures { get; set; } = new List<SpecialFeaturesVM>();
        public IEnumerable<OurServicesVM> OurServices { get; set; } = new List<OurServicesVM>();
        public IEnumerable<PortfolioVM> Portfolio { get; set; } = new List<PortfolioVM>();
        public ContactFormVM ContactForm { get; set; } = new ContactFormVM();
        public IEnumerable<TeamVM> Team { get; set; } = new List<TeamVM>();
        public IEnumerable<PricingVM> Pricing { get; set; } = new List<PricingVM>();
        public IEnumerable<FaqVM> Faq { get; set; } = new List<FaqVM>();
        public IEnumerable<SliderFooterHomeVM> SliderFooterHome { get; set; } = new List<SliderFooterHomeVM>();
        public IEnumerable<NewsVM> News { get; set; } = new List<NewsVM>();
    }
}