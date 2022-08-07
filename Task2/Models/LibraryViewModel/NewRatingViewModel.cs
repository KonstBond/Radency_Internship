using System.ComponentModel.DataAnnotations;

namespace Task2.Models.LibraryViewModel
{
    public class NewRatingViewModel
    {
        [Required]
        [Range(0,5, ErrorMessage = "Score can be from 0 to 5")]
        public int Rating { get; set; }
    }
}
