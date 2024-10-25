using LibApi.Model;
using LibApi.Services.Base;

namespace LibApi.Services.GenreService;

public interface IGenreService : ICrudService<Genre, string>
{
    IReadOnlyList<Genre> GetAll();
}