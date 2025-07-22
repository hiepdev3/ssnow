using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q2.Models;
using Q2.ViewModels; // <- ViewModel ở ngoài thư mục Models

public class MoviesController : Controller
{
    private readonly PePrnFall22B1Context _context;

    public MoviesController(PePrnFall22B1Context context)
    {
        _context = context;
    }

    public IActionResult Director_Movie(int? directorId)
    {
        var query = _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars) 
            .AsQueryable();

        if (directorId.HasValue)
        {
            query = query.Where(m => m.DirectorId == directorId);
        }

        var movieRows = query
            .Select(m => new MovieListViewModel.MovieDisplayRow
            {
                Title = m.Title,
                ReleaseDate = m.ReleaseDate.HasValue
                              ? m.ReleaseDate.Value.ToDateTime(TimeOnly.MinValue)
                              : DateTime.MinValue,
                Description = m.Description ?? "",
                Language = m.Language,
                DirectorName = m.Director != null ? m.Director.FullName : "",
                StarNames = string.Join(",", m.Stars.Select(s => s.FullName.Replace(" ", "")))
            }).ToList();

        var vm = new MovieListViewModel
        {
            SelectedDirectorId = directorId,
            Directors = _context.Directors.ToList(),
            Movies = movieRows
        };

        return View(vm);
    }

}
