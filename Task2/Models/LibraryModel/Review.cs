namespace Task2.Models.LibraryModel
{
    public class Review
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }
        public string Reviewer { get; set; }
    }
}
