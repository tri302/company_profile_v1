using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class PhotoGallery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDPhotoGallery { get; set; }
        public string? UrlGambar { get; set; }
        public DateTime CreateAt { get; set; }
    }
}