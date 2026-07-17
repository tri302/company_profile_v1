using System.ComponentModel.DataAnnotations;

namespace company_profile
{
    public class SliderFooterHomeVM
    {        
        public int Id { get; set; }

        // Pindahkan [Display] jika ini memang nama kolomnya, 
        // tapi HAPUS [Required] dari UrlImage karena ini diisi otomatis dari Controller.
        [Display(Name = "Url Gambar")]
        public string? UrlImage { get; set; }

        // Pindahkan [Required] ke UploadGambar karena saat Create, gambar WAJIB diisi.
        [Display(Name = "Pilih Gambar Slider")]
        // [Required(ErrorMessage = "File gambar wajib diunggah.")]
        public IFormFile? UploadGambar { get; set; }    
    }
}