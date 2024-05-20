using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class FavouriteRepository : IRepository<int, FavouriteModel>
    {
        private ApplicationDbContext context;

        public FavouriteRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(FavouriteModel item)
        {
            context.FavouritesDbSet.Add(item);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var favourite = context.FavouritesDbSet.FirstOrDefault(favourite => favourite.id == id);
            if (favourite != null)
            {
                context.FavouritesDbSet.Remove(favourite);
                context.SaveChanges();
            }
        }


        public FavouriteModel? Get(int id)
        {
            return context.FavouritesDbSet.Find(id);
        }

        public List<FavouriteModel> GetMovieFavourites(int userId)
        {
            return context.FavouritesDbSet.Where(favourite => favourite.type == "movie" && favourite.id == userId).ToList();
        }
        public List<FavouriteModel> GetSerialFavourites(int userId)
        {
            return context.FavouritesDbSet.Where(favourite => favourite.type == "serial" && favourite.id == userId).ToList();
        }

        public List<FavouriteModel> GetAll()
        {
            return context.FavouritesDbSet.ToList();
        }

        public void DeleteAllByTypeAndProductionId(string type, int production_id)
        {
            var favouritesToDelete = context.FavouritesDbSet
                .Where(h => h.type == type && h.production_id == production_id)
                .ToList();

            if (favouritesToDelete.Any())
            {
                context.FavouritesDbSet.RemoveRange(favouritesToDelete);
                context.SaveChanges();
            }
        }

        public bool isExists(int id)
        {
            return context.FavouritesDbSet.Any(history => history.id == id);
        }

        public bool isExistsTotal(int production_id, string type, int user_id)
        {
            return context.FavouritesDbSet.Any(history => history.user_id == user_id && history.type == type && history.production_id == production_id);
        }

        public void DeleteByFavourite(FavouriteModel model)
        {
            var favourite = context.FavouritesDbSet.FirstOrDefault(f => f.production_id == model.production_id && f.type == model.type && f.user_id == model.user_id);
            if (favourite != null)
            {
                context.FavouritesDbSet.Remove(favourite);
                context.SaveChanges();
            }
        }

        public List<FavouriteModel> GetAllByUserId(int userId)
        {
            List<FavouriteModel> favourites = context.FavouritesDbSet
                .Where(h => h.user_id == userId)
                .ToList();
            return favourites;
        }

        public void Update(FavouriteModel item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
