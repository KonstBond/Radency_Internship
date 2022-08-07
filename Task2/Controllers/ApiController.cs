using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Task2.Database;
using Task2.Models.LibraryModel;
using Task2.Models.LibraryViewModel;

namespace Task2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public ApiController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Api/Book?{order=author}
        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<BookViewModel>> Book([FromQuery] string? order = "author")
        {
            return await Task.Run(() =>
            {
                IEnumerable<BookViewModel> result = _context.Books.ToList()
                        .GroupJoin(_context.Ratings,
                        b => b.Id,
                        rat => rat.BookID,
                        (book, rat) => new
                        {
                            Book = book,
                            Rating = rat.Average(rat => rat?.Score) ?? 0
                        })
                        .GroupJoin(_context.Reviews,
                        b => b.Book.Id,
                        rev => rev.BookID,
                        (BookRating, rev) => new BookViewModel
                        {
                            Id = BookRating.Book.Id,
                            Title = BookRating.Book.Title,
                            Author = BookRating.Book.Author,
                            Rating = BookRating.Rating,
                            ReviewsNumber = rev?.Count()
                        }).ToList();

                if (order?.ToLower() == "author")
                {
                    return result.Where(b => b.Author.StartsWith(order))
                                 .OrderBy(b => b.Author.StartsWith(order))
                                 .Concat(result.Where(b => !b.Author.StartsWith(order))
                                                .OrderBy(b => b.Author));
                }
                else if (order?.ToLower() == "title")
                {
                    return result.Where(b => b.Title.StartsWith(order))
                                 .OrderBy(b => b.Title.StartsWith(order))
                                 .Concat(result.Where(b => !b.Title.StartsWith(order))
                                                .OrderBy(b => b.Title));
                }
                else
                    return result;
            });
        }

        // GET: Api/Recommended?{ganre=horror}

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<BookViewModel>> Recommended([FromQuery] string? ganre = "horror")
        {
            return await Task.Run(() =>
            {
                return _context.Books
                       .Where(b => b.Genre == ganre)
                       .ToList()
                       .GroupJoin(_context.Ratings,
                       b => b.Id,
                       rat => rat.BookID,
                       (book, rat) => new
                       {
                           Book = book,
                           Rating = rat.Average(rat => rat?.Score) ?? 0
                       })
                       .GroupJoin(_context.Reviews,
                       b => b.Book.Id,
                       rev => rev.BookID,
                       (BookRating, rev) => new BookViewModel
                       {
                           Id = BookRating.Book.Id,
                           Title = BookRating.Book.Title,
                           Author = BookRating.Book.Author,
                           Rating = BookRating.Rating,
                           ReviewsNumber = rev?.Count()
                       })
                       .Where(b => b.ReviewsNumber > 10)
                       .OrderBy(b => b.Rating)
                       .Take(10);
            });
        }

        // POST api/books/save
        [HttpPost]
        [Route("books/[action]")]
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

        [HttpPut]
        [Route("books/{id}/[action]")]
        public async Task<int> Review(int id, ReviewViewModel reviewViewModel)
        {
            Review review = new Review()
            {
                BookID = id,
                Message = reviewViewModel.Message,
                Reviewer = reviewViewModel.Reviewer
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return review.Id;
        }

        [HttpPut]
        [Route("books/{id}/[action]")]
        public async Task<IActionResult> Rate(int id, NewRatingViewModel newRating)
        {
            if (_context.Books.FirstOrDefault(b => b.Id == id) == null)
                return BadRequest();
            else
            {
                await _context.Ratings.AddAsync(new Rating()
                {
                    BookID = id,
                    Score = newRating.Rating
                });
                return Ok();
            }
        }

        [HttpDelete]
        [Route("books/{id}")]
        public async Task Delete([FromQuery] string? secret = "secret")
        {

        }
    }
}
