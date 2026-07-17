namespace company_profile.PagenationVM
{
    public class SliderFooterListViewModel
    {
        public IEnumerable<SliderFooterHomeVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

