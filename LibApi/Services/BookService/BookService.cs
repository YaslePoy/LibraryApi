using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.BookService;

public class BookService : IBookService
{
    private readonly LibApiContext _libApi;

    public BookService(LibApiContext libApi)
    {
        _libApi = libApi;
    }

    public async Task<int> Create(BookData data)
    {
        var book = Utils.TransferData<Book, BookData>(data);

        await _libApi.Books.AddAsync(book);
        await _libApi.SaveChangesAsync();
        return book.Id;
    }

    public Book Get(int id)
    {
        return _libApi.Books.FirstOrDefault(i => i.Id == id);
    }

    public async Task Update(int id, BookData data)
    {
        var book = Get(id);
        Utils.TransferData(book, data);
        await _libApi.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var book = Get(id);
        _libApi.Books.Remove(book);
        await _libApi.SaveChangesAsync();
    }

    public bool IsExists(int id)
    {
        return _libApi.Books.Any(i => i.Id == id);
    }

    public IReadOnlyList<Book> GetAll()
    {
        return _libApi.Books.ToList();
    }

    public async Task<int> Copy(int id, decimal cost)
    {
        var copy = new BookCopy { BookId = id, Cost = cost, PurchaseDate = DateTime.Now };

        await _libApi.BookCopies.AddAsync(copy);
        await _libApi.SaveChangesAsync();

        return copy.Id;
    }

    public int CopyCount(int id)
    {
        return _libApi.BookCopies.Count(i => i.BookId == id);
    }

    public async Task AddGenre(int bookId, int genreId)
    {
        var rent = new BooksGenre
        {
            BookId = bookId, GenreId = genreId
        };

        await _libApi.BooksGenres.AddAsync(rent);
        await _libApi.SaveChangesAsync();
    }

    public IReadOnlyList<Book> BooksWithGenre(int genreId)
    {
        return _libApi.BooksGenres.Where(i => i.GenreId == genreId).Include(i => i.Book).Select(i => i.Book).ToList();
    }

    public IReadOnlyList<Book> BooksWithAuthor(string author)
    {
        return _libApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Author, author) <= 2).ToList();
    }

    public IReadOnlyList<Book> BooksWithName(string name)
    {
        return _libApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Name, name) <= 2).ToList();
    }

    public IReadOnlyList<BookCopy> AllCopies()
    {
        return _libApi.BookCopies.ToList();
    }

    public BookCopy GetCopy(int copyId)
    {
        return _libApi.BookCopies.Include(i => i.Book).FirstOrDefault(i => i.Id == copyId);
    }

    public bool IsCopyExists(int copyId)
    {
        return _libApi.BookCopies.Any(i => i.Id == copyId);
    }
}