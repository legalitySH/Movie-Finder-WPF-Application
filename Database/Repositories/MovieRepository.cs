using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class MovieRepository : IRepository<int, Movie>
    {
        private ApplicationDbContext context;

        public MovieRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Movie item)
        {
            if (!isExists(item.id))
            {
                context.MoviesDbSet.Add(item);
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var movie = context.MoviesDbSet.FirstOrDefault(movie => movie.id == id);
            if (movie != null)
            {
                context.MoviesDbSet.Remove(movie);
                context.SaveChanges();
            }
        }


        public Movie? Get(int id)
        {
            return context.MoviesDbSet.Find(id);
        }

        public Movie? GetByTitle(string title)
        {
            return context.MoviesDbSet.FirstOrDefault(movie => movie.russian_name == title);
        }

        public List<Movie> GetAll()
        {
            return context.MoviesDbSet.ToList();
        }

        public bool isExists(int id)
        {
            return context.MoviesDbSet.Any(movie => movie.id == id);
        }

        public void Update(Movie item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }

    }
}
