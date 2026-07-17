using Quartz;

namespace company_profile
{
    public class SchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public SchedulerService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task AdjustBroadcastIntervalAsync(int totalSubscribers)
        {
            var strategy = StrategyDeterminer.GetStrategy(totalSubscribers);
            var scheduler = await _schedulerFactory.GetScheduler();

            var triggerKey = new TriggerKey("BroadcastJobTrigger", "Group1");
            var jobKey = new JobKey("BroadcastJob");

            var oldTrigger = await scheduler.GetTrigger(triggerKey);

            // Ambil waktu jam dinding saat ini
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            if (oldTrigger != null)
            {
                // 1. JIKA TRIGGER SUDAH ADA: Buat trigger baru untuk me-reschedule
                var newTrigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x                        
                        .WithIntervalInMinutes(strategy.IntervalMinutes) 
                        .RepeatForever())
                    .Build();

                await scheduler.RescheduleJob(triggerKey, newTrigger);
                Console.WriteLine($"[QUARTZ CONFIG] Sukses RE-SCHEDULE interval menjadi {strategy.IntervalMinutes} menit. Mode: {strategy.Mode}");
            }
            else
            {
                // 2. JIKA TRIGGER BELUM ADA (Fase Startup): Buat langsung dengan mengaitkannya ke ForJob()
                var initialTrigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(jobKey) // <--- Mengaitkan langsung ke Job-nya tanpa extension method
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(strategy.IntervalMinutes) 
                        .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(initialTrigger);
                Console.WriteLine($"[QUARTZ CONFIG] Sukses INITIAL-SCHEDULE interval awal karena subscriber berjumlah {totalSubscribers}. Mode: {strategy.Mode}");
            }
        }
    }
}