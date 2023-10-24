using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationEntity.Models
{
    public class SubPicture
    {
        public int? ID { get; set; }

        public string? Path { get; set; }

        public int? BannerID { get; set; }

    }
}
