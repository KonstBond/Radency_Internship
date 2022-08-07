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
        private readonly ILogger<ApiController> _logger;
        private readonly LibraryDbContext _context;

        public ApiController(LibraryDbContext context, ILogger<ApiController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Book([FromQuery] string? order = "author")
        {
            return await Task.Run<IActionResult>(() =>
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

                    result = result.Where(b => b.Author.StartsWith(order))
                                 .OrderBy(b => b.Author.StartsWith(order))
                                 .Concat(result.Where(b => !b.Author.StartsWith(order))
                                                .OrderBy(b => b.Author));
                    return Ok(result);
                }
                else if (order?.ToLower() == "title")
                {
                    result = result.Where(b => b.Title.StartsWith(order))
                                 .OrderBy(b => b.Title.StartsWith(order))
                                 .Concat(result.Where(b => !b.Title.StartsWith(order))
                                                .OrderBy(b => b.Title));
                }
                
                return Ok(result);
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Recommended([FromQuery] string? ganre = "horror")
        {
            return await Task.Run<IActionResult>(() =>
            {
                var result = _context.Books
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

                return Ok(result);
            });
        }

        [HttpGet]
        [Route("books/{id}")]
        public async Task<IActionResult> BookDetails([FromRoute]int id)
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (_context.Books.FirstOrDefault(b => b.Id == id) is not null)
                {
                    var result = _context.Books
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
                           (BookRating, rev) => new FullInfoBookViewModel
                           {
                               ID = BookRating.Book.Id,
                               Title = BookRating.Book.Title,
                               Author = BookRating.Book.Author,
                               Cover = BookRating.Book.Cover,
                               Content = BookRating.Book.Content,
                               Rating = BookRating.Rating,
                               Reviews = from r in rev
                                         select new FullReviewViewModel
                                         {
                                             Id = r.Id,
                                             Message = r.Message,
                                             Reviewer = r.Reviewer
                                         }

                           }).ToList();

                    return Ok(result);
                }
                else
                {
                    return BadRequest("Not Found this book");
                }
            });
        }

        [HttpPost]
        [Route("books/[action]")]
        public async Task<IActionResult> Save(NewBookViewModel newBookViewModel)
        {
            if (ModelState.IsValid)
            {
                newBookViewModel.Cover = Convert.ToBase64String(Encoding.Unicode.GetBytes(newBookViewModel.Cover));
                Book? DbBook = await _context.Books.FindAsync(new object[] { newBookViewModel.ID });
                if (DbBook is null)
                {
                    await _context.Books.AddAsync(new Book()
                    {
                        Id = 0,
                        Author = newBookViewModel.Author,
                        Content = newBookViewModel.Content,
                        Cover = newBookViewModel.Cover,
                        Genre = newBookViewModel.Genre,
                        Title = newBookViewModel.Title
                    });
                }
                else
                {
                    DbBook.Author = newBookViewModel.Author;
                    DbBook.Title = newBookViewModel.Title;
                    DbBook.Content = newBookViewModel.Content;
                    DbBook.Cover = newBookViewModel.Cover;
                    DbBook.Genre = newBookViewModel.Genre;
                }
                await _context.SaveChangesAsync();
                return Ok(newBookViewModel.ID);
            }
            else
            {
                return BadRequest(ModelState.ErrorCount);
            }
            
        }

        [HttpPut]
        [Route("books/{id}/[action]")]
        public async Task<IActionResult> Review(int id, ReviewViewModel reviewViewModel)
        {
            return await Task.Run<IActionResult>(async () =>
            {
                if (_context.Books.FirstOrDefault(b => b.Id == id) is not null)
                {
                    if (ModelState.IsValid)
                    {
                        Review review = new Review()
                        {
                            BookID = id,
                            Message = reviewViewModel.Message,
                            Reviewer = reviewViewModel.Reviewer
                        };
                        
                        await _context.Reviews.AddAsync(review);
                        await _context.SaveChangesAsync();
                        return Ok(review.Id);
                    }
                    else
                    {
                        return BadRequest(ModelState.ErrorCount);
                    }
                }
                else
                {
                    return BadRequest("Not found a book");
                }
            });
            
            
        }

        [HttpPut]
        [Route("books/{id}/[action]")]
        public async Task<IActionResult> Rate(int id, NewRatingViewModel newRating)
        {
            if (_context.Books.FirstOrDefault(b => b.Id == id) == null)
                return BadRequest("Book is not found");
            else
            {
                if (ModelState.IsValid)
                {
                    await _context.Ratings.AddAsync(new Rating()
                    {
                        BookID = id,
                        Score = newRating.Rating
                    });
                    return Ok();
                }
                else
                {
                    return BadRequest(ModelState.ErrorCount);
                }
            }
        }

        [HttpDelete]
        [Route("books/{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string? secret = "secret")
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (secret == "secret")
                {
                    Book? book = _context.Books.FirstOrDefault(b => b.Id == id);

                    if (book is not null)
                    {
                        _context.Books.Remove(book);
                    }
                    else
                    {
                        return BadRequest("Book is not found");
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest("Secret is not true");
                }
            });
        }
    }
}
