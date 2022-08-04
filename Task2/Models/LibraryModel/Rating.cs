namespace Task2.Models.LibraryModel
{
    public class Rating
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public List<Book> Books { get; set; }
        public int Score { get; set; }
    }
}
