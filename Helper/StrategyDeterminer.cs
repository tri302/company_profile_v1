using company_profile;

namespace company_profile
{
    public static class StrategyDeterminer
    {
        public static BroadcastStrategy GetStrategy(int totalSubscribers)
        {
            // FASE 1: Skala Kecil (Di bawah 2.000 subscriber)
            if (totalSubscribers <= 2000)
            {
                return new BroadcastStrategy
                {
                    BatchSize = totalSubscribers,
                    IntervalMinutes = 5, // Cukup cek setiap 5 menit
                    Mode = "BCC"
                };
            }
            // FASE 2: Skala Menengah (2.001 - 20.000 subscriber)
            else if (totalSubscribers <= 20000)
            {
                return new BroadcastStrategy
                {
                    BatchSize = 500, // Cicil 500 email per jalan
                    IntervalMinutes = 15, // Jalan setiap 15 menit
                    Mode = "Chunking"
                };
            }
            // FASE 3: Skala Raksasa (Di atas 20.000 subscriber)
            else
            {
                return new BroadcastStrategy
                {
                    BatchSize = 2000,
                    IntervalMinutes = 30, // Longgarkan waktu agar antrean di broker aman
                    Mode = "Queue"
                };
            }
        }
    }
}