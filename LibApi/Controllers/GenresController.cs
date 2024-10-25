using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : Controller
{
    // // private readonly LibApiContext _context;
    // private readonly IGenreService _genre;
    //
    // public GenresController(IGenreService genre)
    // {
    //     _genre = genre;
    // }

    [HttpGet("all")]
    public IActionResult GetAllGenres()
    {
        // return Ok(_genre.GetAll());
        return null;

    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateGenre(string name)
    {
        // if (string.IsNullOrWhiteSpace(name))
        //     return BadRequest("Unexpected genre name");
        //
        // var genre = _genre.GetAll().FirstOrDefault(i => i.Name == name);
        // if (genre != null)
        //     return BadRequest("That genre already created");
        //
        // return Ok(new { genreId = await _genre.Create(name) });
        return null;

    }

    [HttpPatch]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> EditGenre(int genreId, string newName)
    {
        // if (string.IsNullOrWhiteSpace(newName))
        //     return BadRequest("Unexpected genre name");
        //
        // if (!_genre.IsExists(genreId))
        //     return NotFound("That genre does not exists");
        //
        // _genre.Update(genreId, newName);

        return Ok();
    }

    [HttpDelete("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteGenre(int genreId)
    {
        // if (!_genre.IsExists(genreId))
        //     return NotFound("That genre does not exists");
        //
        // await _genre.Delete(genreId);

        return Ok();
    }
}