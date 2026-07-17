using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace company_profile
{
    [DisallowConcurrentExecution]
    public class MonitorScaleJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonitorScaleJob> _logger;

        public MonitorScaleJob(IServiceProvider _serviceProvider, ILogger<MonitorScaleJob> logger)
        {
            this._serviceProvider = _serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("[QUARTZ] Memulai pengecekan skala subscriber mingguan/harian...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var schedulerService = scope.ServiceProvider.GetRequiredService<SchedulerService>();

                try
                {
                    // 1. Hitung total subscriber saat ini
                    int totalSubscribers = await dbContext.Subscribers.CountAsync();
                    _logger.LogInformation($"[QUARTZ] Total subscriber saat ini: {totalSubscribers} user.");

                    // 2. Panggil SchedulerService untuk menyesuaikan interval secara otomatis
                    await schedulerService.AdjustBroadcastIntervalAsync(totalSubscribers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[QUARTZ] Gagal menyesuaikan skala interval Quartz secara otomatis.");
                }
            }

            _logger.LogInformation("[QUARTZ] Selesai memantau skala subscriber.");
        }
    }
}