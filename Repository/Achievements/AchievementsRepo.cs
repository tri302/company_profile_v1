using System.Globalization;
using company_profile;
using Microsoft.EntityFrameworkCore;

public class AchievementsRepo : IAchievementsRepo
{
    private readonly AppDbContext _context;

    // Dependency Injection untuk Database Context
    public AchievementsRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CounterVM>> GetAllAsync()
    {
        return await GetQuery().ToListAsync();
    }    

    public async Task<CounterVM?> GetByIdAsync(int id)
    {
        var data = await _context.Counter.FirstOrDefaultAsync(x => x.Id == id);
        string currentLang = CultureInfo.CurrentCulture.Name;
        if (data == null)
        {
            return null;
        }

        return new CounterVM
        {
            Id = data.Id,
            JudulCounter = currentLang.StartsWith("id") ? data.JudulCounter_ID : data.JudulCounter_EN,
            // Pemetaan Kolom Multi-Bahasa Baru
            JudulCounter_ID = data.JudulCounter_ID,
            JudulCounter_EN = data.JudulCounter_EN,
            IsiContentCounter = data.IsiContentCounter
        };
    }

    public async Task<bool> CreateAsync(CounterVM counter)
    {
        var data = new Counter
        {
            // Pemetaan Kolom Multi-Bahasa Baru
            JudulCounter_ID = counter.JudulCounter_ID,
            JudulCounter_EN = counter.JudulCounter_EN,
            IsiContentCounter = counter.IsiContentCounter
        };

        _context.Counter.Add(data);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateAsync(CounterVM counter)
    {
        var data = await _context.Counter.FindAsync(counter.Id);
        if (data == null)
        {
            return false;
        }

        // Perbarui entitas database dengan properti dari ViewModel
        data.JudulCounter_ID = counter.JudulCounter_ID;
        data.JudulCounter_EN = counter.JudulCounter_EN;
        data.IsiContentCounter = counter.IsiContentCounter;

        _context.Counter.Update(data);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var data = await _context.Counter.FindAsync(id);
        if (data == null)
        {
            return false;
        }

        _context.Counter.Remove(data);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<CounterVM>> GetPagedDataAsync(int pageIndex, int pageSize)
    {
        return await GetQuery()
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Counter.CountAsync();
    }

    private IQueryable<CounterVM> GetQuery()
    {
        //Fungsi filter multi bahasa
        string currentLang = CultureInfo.CurrentCulture.Name;
        return _context.Counter.Select(x => new CounterVM
        {
            Id = x.Id,
            JudulCounter = currentLang.StartsWith("id") ? x.JudulCounter_ID : x.JudulCounter_EN,
            // Pemetaan Kolom Multi-Bahasa Baru
            JudulCounter_ID = x.JudulCounter_ID,
            JudulCounter_EN = x.JudulCounter_EN,
            IsiContentCounter = x.IsiContentCounter
        });
    }
}