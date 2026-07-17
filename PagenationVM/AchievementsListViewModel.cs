namespace company_profile.PagenationVM
{
    public class AchievementsListViewModel
    {
        public IEnumerable<CounterVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

