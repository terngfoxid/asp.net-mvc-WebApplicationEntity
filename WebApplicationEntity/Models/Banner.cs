using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationEntity.Models
{
    [MetadataType(typeof(BannerMetadata))]
    public partial class Banner
    {
        public int ID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Path { get; set; }
        public ICollection<SubPicture>? SubPictures { get; set; }

    }

    [MetadataType(typeof(BannerMetadata))]
    public partial class BannerCreate
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Path { get; set; }
        public ICollection<SubPicture>? SubPictures { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

    }

    [MetadataType(typeof(BannerMetadata))]
    public partial class BannerEdit
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Path { get; set; }
        public ICollection<SubPicture>? SubPictures { get; set; }

        public IFormFile? Image { get; set; }

    }

    public class BannerMetadata
    {
        public int ID { get; set; }


        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? Path { get; set; }

        [NotMapped]
        public ICollection<SubPicture> SubPictures { get; set; }
    }

}
