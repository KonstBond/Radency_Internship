using System.ComponentModel.DataAnnotations;
using Task2.Models.LibraryModel;

namespace Task2.Models.LibraryViewModel
{
    public class FullInfoBookViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Cover { get; set; }
        public string Content { get; set; }
        public double Rating { get; set; }
        public IEnumerable<FullReviewViewModel> Reviews { get; set; }
    }
}
