using LibApi.Model;
using LibApi.Requests;
using LibApi.Services.BookService;
using LibApi.Services.GenreService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : Controller
{
    private readonly IBookService _book;
    private readonly IGenreService _genre;

    public BooksController(IBookService book, IGenreService genre)
    {
        _book = book;
        _genre = genre;
    }

    [HttpGet("all")]
    public ActionResult<IReadOnlyList<Book>> GetAllBooks(int page, int pageSize)
    {
        return Ok(_book.GetAll().Chunk(pageSize).ToArray()[page]);
    }

    [HttpGet("filter")]
    public ActionResult<IReadOnlyList<BookData>> GetFiltredBooks(string? author, int? genre, int? year)
    {
        return _book.BooksWithFilter(author, genre, year).Select(Utils.TransferData<BookData, Book>).ToList();
    }

    [HttpGet("{bookId:int}")]
    public ActionResult<BookData> GetBook(int bookId)
    {
        if (!_book.IsExists(bookId))
            return NotFound("No book with that Id");

        return Ok(Utils.TransferData<BookData, Book>(_book.Get(bookId)));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult> CreateBook([FromBody] BookData data)
    {
        if (string.IsNullOrWhiteSpace(data.Author))
            return BadRequest("Unexpected author");

        if (string.IsNullOrWhiteSpace(data.Name))
            return BadRequest("Unexpected book name");

        var book = _book.GetAll().FirstOrDefault(i =>
            i.Name == data.Name && i.Author == data.Author && i.PublicationDate == data.PublicationDate);
        if (book is not null)
            return BadRequest("That book is registered");

        return Ok(await _book.Create(data));
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateBook(int id, [FromBody] BookData request)
    {
        if (string.IsNullOrWhiteSpace(request.Author))
            return BadRequest("Unexpected author");

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Unexpected book name");


        if (!_book.IsExists(id))
            return NotFound($"No book with id {id}");

        await _book.Update(id, request);

        return Ok();
    }

    [HttpDelete("{bookId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteBook(int bookId)
    {
        if (!_book.IsExists(bookId))
            return NotFound($"No user with id {bookId}");

        await _book.Delete(bookId);
        return Ok();
    }

    [HttpPost("{bookId}/release")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> RegisterCopy(int bookId, decimal cost)
    {
        return Ok(new { copyId = await _book.Copy(bookId, cost) });
    }

    [HttpGet("{bookId}/count")]
    public ActionResult CountOfCopies(int bookId)
    {
        if (!_book.IsExists(bookId))

            return NotFound($"No book with id {bookId}");

        return Ok(_book.CopyCount(bookId));
    }

    [HttpPost("{bookId}/genre/{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> AddGenreToBook(int bookId, int genreId)
    {
        if (!_book.IsExists(bookId))
            return NotFound($"No book with id {bookId}");

        if (!_genre.IsExists(genreId))
            return NotFound($"No genre with id {genreId}");

        await _book.AddGenre(bookId, genreId);

        return Ok();
    }

    [HttpGet("genre/{genreId}")]
    public ActionResult<Book> GetBooksByGenre(int genreId, int page, int pageSize)
    {
        if (!_genre.IsExists(genreId))
            return NotFound($"No genre with id {genreId}");

        return Ok(_book.BooksWithGenre(genreId).Chunk(pageSize).ToArray()[page]);
    }

    [HttpGet("author")]
    public ActionResult<IReadOnlyList<Book>> GetBooksByAuthor(string author, int page, int pageSize)
    {
        return Ok(
            _book.BooksWithAuthor(author).Select(Utils.TransferData<BookData, Book>).Chunk(pageSize).ToArray()[page]);
    }

    [HttpGet("name")]
    public ActionResult<IReadOnlyList<Book>> GetBooksByName(string name, int page, int pageSize)
    {
        return Ok(_book.BooksWithName(name).Select(Utils.TransferData<BookData, Book>).Chunk(pageSize).ToArray()[page]);
    }

    [HttpGet("copies")]
    public ActionResult<IReadOnlyList<BookCopy>> GetAllCopies(int page, int pageSize)
    {
        return Ok(_book.AllCopies().Chunk(pageSize).ToArray()[page]);
    }

    [HttpGet("copies/{copyId}")]
    public ActionResult<BookCopy> GetCopy(int copyId)
    {
        return Ok(_book.GetCopy(copyId));
    }
}