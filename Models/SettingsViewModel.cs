using System.Collections.Generic;

namespace company_profile
{
    public class SettingsViewModel
    {
        public SeoMetadata SeoMetadata { get; set; } = new SeoMetadata();
        public Theme Theme { get; set; } = new Theme();

        // Menyimpan data tab Logo & Favicon
        public LogoSetting LogoSetting { get; set; } = new LogoSetting();

        public LogoSetting SettingFavicon { get; set; } = new LogoSetting();

        // Menyimpan data tab Email
        public EmailSetting EmailSetting { get; set; } = new EmailSetting();

        // Menyimpan daftar banner untuk semua halaman (Tab Banner)
        public List<PageBannerSetting> PageBanners { get; set; } = new List<PageBannerSetting>();
        
        // Properti pembantu untuk mendeteksi tab mana yang sedang aktif setelah submit
        public string ActiveTab { get; set; } = "Logo"; 
    }
}