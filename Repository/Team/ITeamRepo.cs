namespace company_profile
{
    public interface ITeamRepo
    {
        //Get All Team
        Task<IEnumerable<TeamVM>> GetAllTeamAsync();

        //Update Team
        Task<bool> UpdateTeamAsync(TeamVM teamVM);

        //Create Team    
        Task<TeamVM> CreateTeamAsync(TeamVM teamVM);

        //Delete Team
        Task<bool> DeleteTeamAsync(int idTeam);

        //Get Team by Id
        Task<TeamVM> GetTeamByIdAsync(int idTeam);

        Task<TeamVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}

