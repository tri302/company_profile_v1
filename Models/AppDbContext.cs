using company_profile.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<SeoMetadata> SeoMetadata { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<LogoSetting> LogoSettings { get; set; }
        public DbSet<EmailSetting> EmailSettings {get; set; }
        public DbSet<PageBannerSetting> PageBanners { get; set; }                
        public DbSet<NewsLog> NewsLog { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<HomeProgressBar> HomeProgressBar { get; set; }
        public DbSet<Header> Header { get; set; }
        public DbSet<BannerHome> BannerHome { get; set; }
        public DbSet<Home> Home { get; set; }
        public DbSet<WhyChooseUs> WhyChooseUs { get; set; }
        public DbSet<SliderFooterHome> SliderFooterHomes { get; set; }
        public DbSet<SpecialFeatures> SpecialFeatures { get; set; }
        public DbSet<Counter> Counter { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }        
        public DbSet<PortfolioGambar> PortfolioGambars { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Kategori> Kategoris { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<OurServices> OurServices { get; set; }
        public DbSet<PhotoGallery> PhotoGalleries { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Otomatis mengubah nama seluruh tabel menjadi huruf kecil (lowercase) saat query dibuat
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToLower());
                }
            }

            // Additional configuration can go here
            modelBuilder.Entity<Testimonial>().ToTable("Testimonials");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<About>().ToTable("Abouts");
            modelBuilder.Entity<Contact>().ToTable("Contacts");
            modelBuilder.Entity<Events>().ToTable("Events");
            modelBuilder.Entity<Faq>().ToTable("Faqs");
            modelBuilder.Entity<Kategori>().ToTable("Kategoris");
            modelBuilder.Entity<News>().ToTable("News");
            modelBuilder.Entity<OurServices>().ToTable("OurServices");
            modelBuilder.Entity<PhotoGallery>().ToTable("PhotoGalleries");
            modelBuilder.Entity<Portfolio>().ToTable("Portfolios");
            modelBuilder.Entity<Pricing>().ToTable("Pricings");

            modelBuilder.Entity<NewsLog>()
            .HasIndex(log => new { log.IDNews, log.Email }); // Membuat index gabungan
            modelBuilder.Entity<News>()
            .HasOne(n => n.Kategori)       // Spesifik ke properti 'Kategori' di class News
            .WithMany(k => k.News)         // Spesifik ke koleksi 'News' di class Kategori
            .HasForeignKey(n => n.IDKategori)
            .OnDelete(DeleteBehavior.Cascade);

            // 🔥 PAKSA EF CORE UNTUK MENYAMBUT IDNews SEBAGAI FOREIGN KEY
            modelBuilder.Entity<NewsLog>()
                .HasOne(nl => nl.News)       // NewsLog memiliki satu News
                .WithMany()                  // News bisa memiliki banyak NewsLog (kosongkan jika tidak ada navigation property di class News)
                .HasForeignKey(nl => nl.IDNews) // Ini Foreign Key-nya!
                .HasPrincipalKey(n => n.IDNews); // Merujuk ke Primary Key ini di tabel News

            // Jika nama tabel kamu di MySQL huruf kecil semua, pastikan juga di sini:
            modelBuilder.Entity<NewsLog>().ToTable("newslog");
        }
    }
}
