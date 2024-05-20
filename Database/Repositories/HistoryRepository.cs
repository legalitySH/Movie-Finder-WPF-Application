using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class HistoryRepository : IRepository<int, HistoryModel>
    {
        private ApplicationDbContext context;

        public HistoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(HistoryModel item)
        {
            if (!isExists(item.id))
            {
                context.HistoryDbSet.Add(item);
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var history = context.HistoryDbSet.FirstOrDefault(history => history.id == id);
            if (history != null)
            {
                context.HistoryDbSet.Remove(history);
                context.SaveChanges();
            }
        }

        public void DeleteAllByTypeAndProductionId(string type, int production_id)
        {
            var historiesToDelete = context.HistoryDbSet
                .Where(h => h.type == type && h.production_id == production_id)
                .ToList();

            if (historiesToDelete.Any())
            {
                context.HistoryDbSet.RemoveRange(historiesToDelete);
                context.SaveChanges();
            }
        }

        public HistoryModel? Get(int id)
        {
            return context.HistoryDbSet.Find(id);
        }

        public List<HistoryModel> GetAll()
        {
            return context.HistoryDbSet.ToList();
        }

        public bool isExists(int id)
        {
            return context.HistoryDbSet.Any(history => history.id == id);
        }

        public bool isExistsTotal(int production_id, string type, int user_id)
        {
            return context.HistoryDbSet.Any(history => history.user_id == user_id && history.type == type && history.production_id == production_id);
        }
        public List<HistoryModel> GetAllByUserId(int userId)
        {
            List<HistoryModel> historyList = context.HistoryDbSet
                .Where(h => h.user_id == userId)
                .ToList();
            return historyList;
        }

        public List<HistoryModel> GetMoviesHistory(int userId)
        {
            return context.HistoryDbSet.Where(history => history.type == "movie" && history.user_id == userId).ToList();
        }

        public List<HistoryModel> GetSerialsHistory(int userId)
        {
            return context.HistoryDbSet.Where(history => history.type == "serial" && history.user_id == userId).ToList();
        }

        public void Update(HistoryModel item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
