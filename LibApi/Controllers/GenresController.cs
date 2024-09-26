using LibApi.DataBaseContext;
using LibApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : Controller
{
    private readonly LibApiContext _context;

    public GenresController(LibApiContext context)
    {
        _context = context;
    }

    [HttpGet("all")]
    public IActionResult GetAllGenres()
    {
        return Ok(_context.Genres.ToList());
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateGenre(string name)
    {
        var genre = _context.Genres.FirstOrDefault(i => i.Name == name);
        if (genre != null)
            return BadRequest("That genre already created");

        genre = new Genre
        {
            Name = name
        };

        await _context.Genres.AddAsync(genre);
        await _context.SaveChangesAsync();

        return Ok(new { genreId = genre.Id });
    }

    [HttpPatch]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> EditGenre(int genreId, string newName)
    {
        var genre = _context.Genres.FirstOrDefault(i => i.Id == genreId);
        if (genre is null)
            return NotFound("That genre does not exists");

        genre.Name = newName;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteGenre(int genreId)
    {
        var genre = _context.Genres.FirstOrDefault(i => i.Id == genreId);
        if (genre is null)
            return NotFound("That genre does not exists");

        _context.Remove(genre);
        await _context.SaveChangesAsync();
        return Ok();
    }
}