using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult GetAllBooks()
    {
        return Ok(_libApi.Books.ToList());
    }

    [HttpGet("{bookId:int}")]
    public ActionResult GetBook(int bookId)
    {
        var book = _libApi.Books.FirstOrDefault(i => i.Id == bookId);
        if (book is null)
            return BadRequest("There is no book with that Id");

        return Ok(Utils.TransferData<BookData, Book>(book));
    }

    [HttpPost]
    public async Task<ActionResult> CreateBook([FromBody] BookData data)
    {
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
    public async Task<ActionResult> UpdateBook(int id, [FromBody] BookData request)
    {
        var user = _libApi.Books.FirstOrDefault(i => i.Id == id);

        if (user is null)
            return BadRequest($"No book with id {id}");

        Utils.TransferData(user, request);

        await _libApi.SaveChangesAsync();

        return Ok();
    }
    
    [HttpDelete("{bookId}")]
    public async Task<ActionResult> DeleteBook(int bookId)
    {
        var user = _libApi.Books.FirstOrDefault(i => i.Id == bookId);

        if (user is null)
            return BadRequest($"No user with id {bookId}");

        _libApi.Books.Remove(user);
        await _libApi.SaveChangesAsync();
        return Ok();
    }
}