namespace company_profile.PagenationVM
{
    public class SpecialFeaturesListViewModel
    {
        public IEnumerable<SpecialFeaturesVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

