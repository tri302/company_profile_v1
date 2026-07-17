namespace company_profile
{
    public interface IAchievementsRepo
    {
        // Mengambil semua data counter
        Task<IEnumerable<CounterVM>> GetAllAsync();

        // Mengambil satu data counter berdasarkan ID
        Task<CounterVM?> GetByIdAsync(int id);

        // Menambah data counter baru
        Task<bool> CreateAsync(CounterVM counter);

        // Memperbarui data counter yang sudah ada
        Task<bool> UpdateAsync(CounterVM counter);

        // Menghapus data counter berdasarkan ID
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<CounterVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}
