using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Task2.Database;
using Task2.Models.LibraryModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }


        // GET: api/<HomeController>/{author}
        [HttpGet]
        [Route("{author?}")]
        public async Task<IEnumerable> GetAll([FromRoute] string? author)
        {
            return await Task.Run<IEnumerable>(() =>
            {
                if (author is null)
                {
                    return _context.Books.ToList()
                        .GroupJoin(_context.Ratings,
                        b => b.Id,
                        rat => rat.BookId,
                        (book, rat) => new
                        {
                            Book = book,
                            Rating = rat.Average(rat => rat?.Score) ?? 0
                        })
                        .GroupJoin(_context.Reviews,
                        b => b.Book.Id,
                        rev => rev.BookId,
                        (BookRating, rev) => new
                        {
                            Id = BookRating.Book.Id,
                            Title = BookRating.Book.Title,
                            Author = BookRating.Book.Author,
                            Rating = BookRating.Rating,
                            ReviewsNumber = rev?.Count()
                        }).ToList();
                }
                else
                {
                    var result = _context.Books.ToList()
                        .GroupJoin(_context.Ratings,
                        b => b.Id,
                        rat => rat.BookId,
                        (book, rat) => new
                        {
                            Book = book,
                            Rating = rat.Average(rat => rat?.Score) ?? 0
                        })
                        .GroupJoin(_context.Reviews,
                        b => b.Book.Id,
                        rev => rev.BookId,
                        (BookRating, rev) => new
                        {
                            Id = BookRating.Book.Id,
                            Title = BookRating.Book.Title,
                            Author = BookRating.Book.Author,
                            Rating = BookRating.Rating,
                            ReviewsNumber = rev?.Count()
                        }).ToList();

                    return result.Where(b => b.Author.StartsWith(author))
                          .OrderBy(b => b.Author.StartsWith(author))
                          .Concat(result.Where(b => !b.Author.StartsWith(author))
                                        .OrderBy(b => b.Author));
                }
            });
        }

        // POST api/<HomeController>
        [HttpPost]
        public async Task<int> Save(Book book)
        {
            book.Cover = Convert.ToBase64String(Encoding.Unicode.GetBytes(book.Cover));
            Book? DbBook = await _context.FindAsync<Book>(new object[] { book.Id });
            if (DbBook is null)
            {
                book.Id = 0;
                await _context.AddAsync(book);
            }
            else
            {
                DbBook.Author = book.Author;
                DbBook.Title = book.Title;
                DbBook.Content = book.Content;
                DbBook.Cover = book.Cover;
                DbBook.Genre = book.Genre;
            }
            await _context.SaveChangesAsync();
            return book.Id;
        }
    }
}
