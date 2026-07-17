using Serilog;
using Quartz;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using company_profile;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using company_profile.Helper;
using company_profile.Middleware;
using company_profile.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<DatabaseKeepAliveService>();

// Paksa aplikasi menyimpan kunci enkripsi di folder proyek, bukan di sistem OS hosting
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "PersistedKeys")));

//Konfigurasi logger
// handle global error handling pakai Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AutoSeoFilter>();
})
.AddViewLocalization()
.AddDataAnnotationsLocalization()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Tambahkan service session
// Untuk simpan captcha, atau data sementara lain yang dibutuhkan antar request
// disimpan di sesion server, bukan di cookie. Jadi lebih aman & bisa simpan data kompleks seperti object atau list.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Tingkatkan batas ukuran file upload untuk Events (misal: 100MB)
builder.WebHost.ConfigureKestrel(options =>
{
    // Beri sedikit ruang agar server tetap bisa memproses error message
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

// Daftarkan Repository
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings")); // handle global error handling
builder.Services.AddHostedService<FileCleanupWorker>();
builder.Services.AddScoped<IImageCleanupService, ImageCleanupService>();
builder.Services.AddSingleton<SchedulerService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<INewsletterRepo, NewsletterRepo>();
builder.Services.AddScoped<ISliderFooterRepo, SliderFooterRepo>();
builder.Services.AddScoped<IIndexHome, IndexHome>();
builder.Services.AddScoped<IAchievementsRepo, AchievementsRepo>();
builder.Services.AddScoped<ISpecialFeaturesRepo, SpecialFeaturesRepo>();
builder.Services.AddScoped<IWhyChooseUsRepo, WhyChooseUsRepo>();
builder.Services.AddScoped<IHomeRepo, HomeRepo>();
builder.Services.AddScoped<IBannerHomeRepo, BannerHomeRepo>();
builder.Services.AddScoped<IHeaderRepo, HeaderRepo>();
builder.Services.AddScoped<IUserContactedRepo, UserContactedRepo>();
builder.Services.AddScoped<IAboutRepo, AboutRepo>();
builder.Services.AddScoped<IContactRepo, ContactRepo>();
builder.Services.AddScoped<IEventsRepo, EventsRepo>();
builder.Services.AddScoped<IFaqRepo, FaqRepo>();
builder.Services.AddScoped<IKategoriRepo, KategoriRepo>();
builder.Services.AddScoped<IOurServicesRepo, OurServicesRepo>();
builder.Services.AddScoped<IPhotoGaleryRepo, PhotoGaleryRepo>();
builder.Services.AddScoped<IPortfolioRepo, PortfolioRepo>();
builder.Services.AddScoped<IPricingRepo, PricingRepo>();
builder.Services.AddScoped<ITeamRepo, TeamRepo>();
builder.Services.AddScoped<ITestimonialRepo, TestimonialRepo>();
builder.Services.AddScoped<INewsRepo, NewsRepo>();

// Register Quartz.NET
// Fungsi untuk broadcast berita kesemua email subcriber secara otomatis
builder.Services.AddQuartz(q =>
{
    // ==========================================
    // 1. REGISTRASI BROADCAST JOB (Job Utama Anda)
    // ==========================================
    var broadcastJobKey = new JobKey("BroadcastJob");
    q.AddJob<BroadcastJob>(opts => opts.WithIdentity(broadcastJobKey));
    q.AddTrigger(opts => opts
        .ForJob(broadcastJobKey)
        .WithIdentity("BroadcastJobTrigger", "Group1")
        // Default awal misal 15 menit, nanti akan diubah dinamis oleh MonitorScaleJob
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(15).RepeatForever())
        .StartNow());

    // ==========================================
    // 2. REGISTRASI MONITOR SCALE JOB (Job Baru)
    // ==========================================
    var monitorJobKey = new JobKey("MonitorScaleJob");
    q.AddJob<MonitorScaleJob>(opts => opts.WithIdentity(monitorJobKey));
    q.AddTrigger(opts => opts
        .ForJob(monitorJobKey)
        .WithIdentity("MonitorScaleJobTrigger", "Group1")
// Menggunakan Cron Expression untuk berjalan setiap TENGAH MALAM (Jam 00:00)
.WithCronSchedule("0 0 0 * * ?"));
// Menggunakan Cron Expression untuk berjalan setiap 1 MENIT untuk keperluan testing
//.WithCronSchedule("0 * * * * ?"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    );
});

//Configure Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password policy
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // Lockout
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        // User
        options.User.RequireUniqueEmail = true;

        // Signin
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Gunakan PostConfigure untuk mengonfigurasi Cookie yang dibuat oleh Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error/Forbidden";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);

    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // UBAH BAGIAN INI: Jangan Always, ganti jadi SameAsRequest
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    // UBAH BAGIAN INI: Dari Strict ganti ke Lax agar redirect antar domain/proxy tidak memblokir cookie
    options.Cookie.SameSite = SameSiteMode.Lax;
    //options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddMemoryCache();

// ⚠️ Tambahkan ini hanya jika pakai login
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Konfigurasi Middleware Lokalisasi
var supportedCultures = new[] {
    new CultureInfo("id-ID"), // Bahasa Indonesia
    new CultureInfo("en-US")  // English (US)
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("id-ID"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders.Clear();

localizationOptions.RequestCultureProviders.Add(
    new CookieRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

// Konfigurasi otomatis interval berdasarkan jumlah subscriber saat aplikasi pertama start
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var schedulerService = scope.ServiceProvider.GetRequiredService<SchedulerService>();

    int currentSubscribers = await dbContext.Subscribers.CountAsync();
    await schedulerService.AdjustBroadcastIntervalAsync(currentSubscribers);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}"); // handle global error handling
app.UseExceptionHandler("/Error/ServerError"); // handle global error handling
app.UseMiddleware<GlobalExceptionMiddleware>(); // handle global error handling
app.UseMiddleware<MaintenanceMiddleware>(); // handle global error handling
app.UseHttpsRedirection();
app.UseStaticFiles();

// Posisikan Routing paling pertama dari sirkuit validasi URL
app.UseRouting();
// Jalankan Session setelah Routing
app.UseSession();

// Jalankan Autentikasi (Mengenali siapa yang login)
app.UseAuthentication();
// Jalankan Otorisasi (Mengecek role / hak akses)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Company}/{action=Home}/{id?}");

// Gunakan scope untuk mengakses service
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Panggil seeder
        await DbSeeder.SeedSuperAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Menangkap error jika seeding gagal
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Terjadi kesalahan saat menjalankan Database Seeder.");
    }
}


app.Run();
