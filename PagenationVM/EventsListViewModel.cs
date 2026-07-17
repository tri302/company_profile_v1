namespace company_profile.PagenationVM
{
    public class EventsListViewModel
    {
        public IEnumerable<EventsVM> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

