using DevExpress.Mvvm.Native;
using MovieFinder.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MovieFinder.Attributes
{
    public static class MovieFinderSearcher
    {

        public static ObservableCollection<Movie> GetResultOfMovieSearch(ObservableCollection<Movie> movies, string query)
        {
            Regex regex = new Regex(query, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return movies.Where(movie => regex.IsMatch(movie.russian_name ?? string.Empty) || regex.IsMatch(movie.russian_name ?? string.Empty)).ToObservableCollection();
        }

        public static ObservableCollection<Serial> GetResultOfSerialSearch(ObservableCollection<Serial> movies, string query)
        {
            Regex regex = new Regex(query, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return movies.Where(serial => regex.IsMatch(serial.title ?? string.Empty)).ToObservableCollection();
        }
    }
}
