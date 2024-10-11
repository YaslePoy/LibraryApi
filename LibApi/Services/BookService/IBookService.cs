using LibApi.Model;
using LibApi.Requests;
using LibApi.Services.Base;

namespace LibApi.Services.BookService;

public interface IBookService : ICrudService<Book, BookData>
{
    IReadOnlyList<Book> GetAll();
    Task<int> Copy(int id, decimal cost);
    int CopyCount(int id);

    Task AddGenre(int bookId, int genreId);
    IReadOnlyList<Book> BooksWithGenre(int genreId);
    IReadOnlyList<Book> BooksWithAuthor(string author);
    IReadOnlyList<Book> BooksWithName(string name);
    IReadOnlyList<Book> BooksWithFilter(string? author, int? genre, int? year);
    
    IReadOnlyList<BookCopy> AllCopies();
    BookCopy GetCopy(int copyId);
    bool IsCopyExists(int copyId);
}