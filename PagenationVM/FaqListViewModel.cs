namespace company_profile.PagenationVM
{
    public class FaqListViewModel
    {
        public IEnumerable<FaqVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
