

namespace company_profile
{
    public interface IEventsRepo
    {
        //Get All Events
        Task<IEnumerable<EventsVM>> GetAllEventsAsync();

        //Update Events
        Task<bool> UpdateEventsAsync(EventsVM eventsVM);

        //Create Events    
        Task<EventsVM> CreateEventsAsync(EventsVM eventsVM);

        //Delete Events
        Task<bool> DeleteEventsAsync(int idEvents);

        //Get Events by Id
        Task<EventsVM> GetEventsByIdAsync(int idEvents);

        Task<IEnumerable<EventsVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}

