using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class SeoMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string MetaKeywords { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string OgImageUrl { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? NewFoto { get; set; }
    }
}