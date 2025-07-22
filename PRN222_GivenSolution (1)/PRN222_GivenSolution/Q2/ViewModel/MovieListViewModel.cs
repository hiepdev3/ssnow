using Q2.Models;

namespace Q2.ViewModels
{
    public class MovieListViewModel
    {
        public int? SelectedDirectorId { get; set; }
        public List<Director> Directors { get; set; }

        public List<MovieDisplayRow> Movies { get; set; }

        public class MovieDisplayRow
        {
            public string Title { get; set; }
            public DateTime ReleaseDate { get; set; }
            public string Description { get; set; }
            public string Language { get; set; }
            public string DirectorName { get; set; }
            public string StarNames { get; set; }
        }
    }
}
