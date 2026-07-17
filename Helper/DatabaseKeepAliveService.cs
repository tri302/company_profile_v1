using MySqlConnector;

namespace company_profile
{
    public class DatabaseKeepAliveService : BackgroundService
    {
        private readonly ILogger<DatabaseKeepAliveService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // Mengubah interval menjadi 5 menit agar tidak keburu di-timeout oleh MySQL/Aiven
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public DatabaseKeepAliveService(ILogger<DatabaseKeepAliveService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            //Nilai connection string dimasukkan ke variabel _connectionString
            //_connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connectionString = "Server=mysql-3f5e2aa6-democp636-0a65.e.aivencloud.com;Port=14309;Database=defaultdb;Uid=avnadmin;Pwd=AVNS_OFuVRKlCzVC1zSK61wO;SslMode=None;AllowPublicKeyRetrieval=True;";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Database Keep-Alive Service telah dimulai.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Mengirim ping ke MySQL Aiven pada: {time}", DateTimeOffset.Now);

                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        await connection.OpenAsync(stoppingToken);

                        using (var command = new MySqlCommand("SELECT 1;", connection))
                        {
                            await command.ExecuteScalarAsync(stoppingToken);
                        }
                    }

                    _logger.LogInformation("Ping Berhasil. Database tetap terjaga.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengirim ping ke database.");
                }

                // Menunggu sesuai interval sebelum eksekusi berikutnya
                // Task.Delay otomatis menangani stoppingToken jika aplikasi dihentikan
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Memastikan log keluar dengan bersih saat aplikasi stop
                    break;
                }
            }

            _logger.LogInformation("Database Keep-Alive Service telah dihentikan.");
        }
    }
}