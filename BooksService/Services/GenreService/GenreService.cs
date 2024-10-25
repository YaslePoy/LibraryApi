using LibApi.DataBaseContext;
using LibApi.Model;

namespace LibApi.Services.GenreService;

public class GenreService : IGenreService
{
    private readonly BooksApiContext _context;

    public GenreService(BooksApiContext context)
    {
        _context = context;
    }

    public async Task<int> Create(string data)
    {
        var genre = new Genre
        {
            Name = data
        };

        await _context.Genres.AddAsync(genre);
        await _context.SaveChangesAsync();
        return genre.Id;
    }

    public Genre Get(int id)
    {
        return _context.Genres.FirstOrDefault(i => i.Id == id);
    }

    public async Task Update(int id, string data)
    {
        var genre = _context.Genres.FirstOrDefault(i => i.Id == id);
        genre.Name = data;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var genre = _context.Genres.FirstOrDefault(i => i.Id == id);
        _context.Remove(genre);
        await _context.SaveChangesAsync();
    }

    public bool IsExists(int id)
    {
        return _context.Genres.Any(i => i.Id == id);
    }

    public IReadOnlyList<Genre> GetAll()
    {
        return _context.Genres.ToList();
    }
}