using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace company_profile
{
    public class PhotoGalleryVM
    {
        public int IDPhotoGallery { get; set; }
        public string? UrlGambar { get; set; }
        public IFormFile? UploadGambar { get; set; }
        public DateTime CreateAt { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }

        [ValidateNever]
        public int TotalPages { get; set; }

        [ValidateNever]
        public IEnumerable<PhotoGalleryVM>? ListOurPhotoGallery { get; set; } = new List<PhotoGalleryVM>();
    }
}