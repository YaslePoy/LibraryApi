using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : Controller
{
    private readonly LibApiContext _libApi;

    public BooksController(LibApiContext libApi)
    {
        _libApi = libApi;
    }

    [HttpGet("all")]
    public ActionResult<IReadOnlyList<Book>> GetAllBooks()
    {
        return Ok(_libApi.Books.ToList());
    }

    [HttpGet("{bookId:int}")]
    public ActionResult<BookData> GetBook(int bookId)
    {
        var book = _libApi.Books.FirstOrDefault(i => i.Id == bookId);
        if (book is null)
            return NotFound("No book with that Id");

        return Ok(Utils.TransferData<BookData, Book>(book));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult> CreateBook([FromBody] BookData data)
    {
        if (string.IsNullOrWhiteSpace(data.Author))
            return BadRequest("Unexpected author");

        if (string.IsNullOrWhiteSpace(data.Name))
            return BadRequest("Unexpected book name");

        var book = _libApi.Books.FirstOrDefault(i =>
            i.Name == data.Name && i.Author == data.Author && i.PublicationDate == data.PublicationDate);
        if (book is not null)
            return BadRequest("That book is registered");

        book = Utils.TransferData<Book, BookData>(data);

        await _libApi.Books.AddAsync(book);
        await _libApi.SaveChangesAsync();
        return Ok(book.Id);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateBook(int id, [FromBody] BookData request)
    {
        if (string.IsNullOrWhiteSpace(request.Author))
            return BadRequest("Unexpected author");

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Unexpected book name");

        var book = _libApi.Books.FirstOrDefault(i => i.Id == id);

        if (book is null)
            return NotFound($"No book with id {id}");

        Utils.TransferData(book, request);

        await _libApi.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{bookId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteBook(int bookId)
    {
        var book = _libApi.Books.FirstOrDefault(i => i.Id == bookId);

        if (book is null)
            return NotFound($"No user with id {bookId}");

        _libApi.Books.Remove(book);
        await _libApi.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{bookId}/release")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> RegisterCopy(int bookId, decimal cost)
    {
        var copy = new BookCopy { BookId = bookId, Cost = cost, PurchaseDate = DateTime.Now };

        await _libApi.BookCopies.AddAsync(copy);
        await _libApi.SaveChangesAsync();

        return Ok(new { copyId = copy.Id });
    }

    [HttpGet("{bookId}/count")]
    public ActionResult CountOfCopies(int bookId)
    {
        var book = _libApi.Books.FirstOrDefault(i => i.Id == bookId);

        if (book is null)
            return NotFound($"No book with id {bookId}");

        return Ok(_libApi.BookCopies.Count(i => i.BookId == bookId));
    }

    [HttpPost("{bookId}/genre/{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> AddGenreToBook(int bookId, int genreId)
    {
        var book = _libApi.Books.FirstOrDefault(i => i.Id == bookId);
        if (book is null)
            return NotFound($"No book with id {bookId}");

        var genre = _libApi.Genres.FirstOrDefault(i => i.Id == genreId);
        if (genre is null)
            return NotFound($"No genre with id {genreId}");

        var rent = new BooksGenre
        {
            BookId = bookId, GenreId = genreId
        };

        await _libApi.BooksGenres.AddAsync(rent);
        await _libApi.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("genre/{genreId}")]
    public ActionResult<Book> GetBooksByGenre(int genreId)
    {
        var genre = _libApi.Genres.FirstOrDefault(i => i.Id == genreId);
        if (genre is null)
            return NotFound($"No genre with id {genreId}");

        return Ok(_libApi.BooksGenres.Where(i => i.GenreId == genreId).Include(i => i.Book).Select(i => i.Book));
    }

    [HttpGet("author")]
    public ActionResult<IReadOnlyList<Book>> GetBooksByAuthor(string author)
    {
        return Ok(_libApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Author, author) <= 2)
            .Select(Utils.TransferData<BookData, Book>));
    }

    [HttpGet("name")]
    public ActionResult<IReadOnlyList<Book>> GetBooksByName(string name)
    {
        return Ok(_libApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Name, name) <= 2)
            .Select(Utils.TransferData<BookData, Book>));
    }

    [HttpGet("copies")]
    public ActionResult<IReadOnlyList<BookCopy>> GetAllCopies()
    {
        return Ok(_libApi.BookCopies.Include(i => i.Book).ToList());
    }

    [HttpGet("copies/{copyId}")]
    public ActionResult<BookCopy> GetCopy(int copyId)
    {
        return Ok(_libApi.BookCopies.Include(i => i.Book).FirstOrDefault(i => i.Id == copyId));
    }
}