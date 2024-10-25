using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.BookService;

public class BookService : IBookService
{
    private readonly BooksApiContext _booksApi;

    public BookService(BooksApiContext booksApi)
    {
        _booksApi = booksApi;
    }

    public async Task<int> Create(BookData data)
    {
        var book = Utils.TransferData<Book, BookData>(data);

        await _booksApi.Books.AddAsync(book);
        await _booksApi.SaveChangesAsync();
        return book.Id;
    }

    public Book Get(int id)
    {
        return _booksApi.Books.FirstOrDefault(i => i.Id == id);
    }

    public async Task Update(int id, BookData data)
    {
        var book = Get(id);
        Utils.TransferData(book, data);
        await _booksApi.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var book = Get(id);
        _booksApi.Books.Remove(book);
        await _booksApi.SaveChangesAsync();
    }

    public bool IsExists(int id)
    {
        return _booksApi.Books.Any(i => i.Id == id);
    }

    public IReadOnlyList<Book> GetAll()
    {
        return _booksApi.Books.ToList();
    }

    public async Task<int> Copy(int id, decimal cost)
    {
        var copy = new BookCopy { BookId = id, Cost = cost, PurchaseDate = DateTime.Now };

        await _booksApi.BookCopies.AddAsync(copy);
        await _booksApi.SaveChangesAsync();

        return copy.Id;
    }

    public int CopyCount(int id)
    {
        return _booksApi.BookCopies.Count(i => i.BookId == id);
    }

    public async Task AddGenre(int bookId, int genreId)
    {
        var rent = new BooksGenre
        {
            BookId = bookId, GenreId = genreId
        };

        await _booksApi.BooksGenres.AddAsync(rent);
        await _booksApi.SaveChangesAsync();
    }

    public IReadOnlyList<Book> BooksWithGenre(int genreId)
    {
        return _booksApi.BooksGenres.Where(i => i.GenreId == genreId).Include(i => i.Book).Select(i => i.Book).ToList();
    }

    public IReadOnlyList<Book> BooksWithAuthor(string author)
    {
        return _booksApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Author, author) <= 2).ToList();
    }

    public IReadOnlyList<Book> BooksWithName(string name)
    {
        return _booksApi.Books.ToList().Where(i => Utils.LevenshteinDistance(i.Name, name) <= 2).ToList();
    }

    public IReadOnlyList<Book> BooksWithFilter(string? author, int? genre, int? year)
    {
        return _booksApi.BooksGenres.Include(i => i.Book)
            .Where(i => (genre == null || i.GenreId == genre) && (author == null || Utils.LevenshteinDistance(i.Book.Author, author) <= 2) && (year == null || i.Book.PublicationDate.Year == year)).ToList().Select(i => i.Book).ToList();
    }

    public IReadOnlyList<BookCopy> AllCopies()
    {
        return _booksApi.BookCopies.ToList();
    }

    public BookCopy GetCopy(int copyId)
    {
        return _booksApi.BookCopies.Include(i => i.Book).FirstOrDefault(i => i.Id == copyId);
    }

    public bool IsCopyExists(int copyId)
    {
        return _booksApi.BookCopies.Any(i => i.Id == copyId);
    }
}