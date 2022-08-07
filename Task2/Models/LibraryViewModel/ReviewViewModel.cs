using System.ComponentModel.DataAnnotations;

namespace Task2.Models.LibraryViewModel
{
    public class ReviewViewModel
    {
        [Required]
        public string Message { get; set; }
        [Required]
        public string Reviewer { get; set; }
    }
}
