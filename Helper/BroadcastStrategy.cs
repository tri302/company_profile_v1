namespace company_profile
{
    public class BroadcastStrategy
    {
        public int BatchSize { get; set; }      // Jumlah email yang diambil per satu kali jalan
        public int IntervalMinutes { get; set; } // Interval Quartz harus diubah ke berapa menit
        public string Mode { get; set; } = "BCC"; // BCC, Chunking, atau Queue
    }
}