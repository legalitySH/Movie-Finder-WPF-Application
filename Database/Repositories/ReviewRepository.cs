using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private ApplicationDbContext context;

        public ReviewRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public void Add(Review item)
        {
            context.ReviewsDbSet.Add(item);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var review = context.ReviewsDbSet.FirstOrDefault(review => review.id == id);
            if (review != null)
            {
                context.ReviewsDbSet.Remove(review);
                context.SaveChanges();
            }
        }

        public void DeleteAllByTypeAndProductionId(string type, int production_id)
        {
            var reviewsToDelete = context.ReviewsDbSet
                .Where(r => r.type == type && r.production_id == production_id)
                .ToList();

            if (reviewsToDelete.Any())
            {
                context.ReviewsDbSet.RemoveRange(reviewsToDelete);
                context.SaveChanges();
            }
        }


        public Review? Get(int id)
        {
            return context.ReviewsDbSet.Find(id);
        }

        public List<Review> GetAll()
        {

            return context.ReviewsDbSet.ToList().OrderByDescending(review => review.id).ToList();
        }

        public bool isExists(int id)
        {
            return context.ReviewsDbSet.Any(review => review.id == id);
        }

        public bool isExistsTotal(int production_id, string type, int user_id)
        {
            return context.ReviewsDbSet.Any(review => review.user_id == user_id && review.type == type && review.production_id == production_id);
        }

        public void DeleteByReview(Review model)
        {
            var favourite = context.ReviewsDbSet.FirstOrDefault(r => r.production_id == model.production_id && r.type == model.type && r.user_id == model.user_id && r.review_text == model.review_text);
            if (favourite != null)
            {
                context.ReviewsDbSet.Remove(favourite);
                context.SaveChanges();
            }
        }

        public List<Review> GetAllByProduction(string type, int production_id)
        {
            if (GetAll().Count != 0)
            {
                return context.ReviewsDbSet
.Where(r => r.type == type && r.production_id == production_id)
.OrderByDescending(review => review.id)
.ToList();
            }
            return new List<Review>();
        }

        public void Update(Review item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
