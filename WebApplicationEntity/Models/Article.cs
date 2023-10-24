using System.ComponentModel.DataAnnotations;

namespace WebApplicationEntity.Models
{
    public class Article
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }

        public DateTime? Time { get; set; }
    }
}
