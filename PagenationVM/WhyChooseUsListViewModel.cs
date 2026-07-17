using company_profile.ViewModels;

namespace company_profile.PagenationVM
{
    public class WhyChooseUsListViewModel
    {
        public IEnumerable<WhyChooseUsVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

