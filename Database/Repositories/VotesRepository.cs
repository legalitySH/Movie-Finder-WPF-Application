using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class VotesRepository : IRepository<int, VoteModel>
    {
        private ApplicationDbContext context;

        public VotesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(VoteModel item)
        {
            context.VotesDbSet.Add(item);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var vote = context.VotesDbSet.FirstOrDefault(vote => vote.id == id);
            if (vote != null)
            {
                context.VotesDbSet.Remove(vote);
                context.SaveChanges();
            }
        }

        public VoteModel? Get(int id)
        {
            return context.VotesDbSet.Find(id);
        }

        public VoteModel? GetUserVote(string type, int production_id, int user_id)
        {
            return context.VotesDbSet.FirstOrDefault(vote => vote.type == type && vote.production_id == production_id && vote.user_id == user_id);
        }

        public void DeleteAllByTypeAndProductionId(string type, int production_id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var votesToDelete = context.VotesDbSet
                        .Where(v => v.type == type && v.production_id == production_id)
                        .ToList();

                    if (votesToDelete.Any())
                    {
                        context.VotesDbSet.RemoveRange(votesToDelete);
                        context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<VoteModel>? GetAllByTypeAndProduction(string type, int production_id)
        {
            return context.VotesDbSet.Where(vote => vote.type == type && vote.production_id == production_id).ToList();
        }
        public List<VoteModel> GetAll()
        {
            return context.VotesDbSet.ToList();
        }

        public List<VoteModel> GetAllByTypeAndProductionId(string type, int production_id)
        {
            return context.VotesDbSet.Where(view => view.type == type && view.production_id == production_id).ToList();
        }

        public bool isExists(int id)
        {
            return context.VotesDbSet.Any(vote => vote.id == id);
        }

        public bool isExistsTotal(int production_id, string type, int user_id)
        {
            return context.VotesDbSet.Any(vote => vote.user_id == user_id && vote.type == type && vote.production_id == production_id);
        }

        public void Update(VoteModel item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
