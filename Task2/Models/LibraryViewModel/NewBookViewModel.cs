using System.ComponentModel.DataAnnotations;

namespace Task2.Models.LibraryViewModel
{
    public class NewBookViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Cover { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Author { get; set; }
    }
}
