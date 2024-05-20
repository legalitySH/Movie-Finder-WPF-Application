using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class UsersRepository : IRepository<int, User>
    {
        private ApplicationDbContext context;

        public UsersRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(User item)
        {
            if (!isExists(item.id))
            {
                context.UsersDbSet.Add(item);
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var user = context.UsersDbSet.FirstOrDefault(User => User.id == id);
            if (user != null)
            {
                context.UsersDbSet.Remove(user);
                context.SaveChanges();
            }
        }

        public User? Get(int id)
        {
            return context.UsersDbSet.Find(id);
        }

        public User? GetByLogin(string login)
        {
            return context.UsersDbSet.FirstOrDefault(User => User.login == login);
        }

        public List<User> GetAll()
        {
            return context.UsersDbSet.ToList();
        }

        public bool isExists(int id)
        {
            return context.UsersDbSet.Any(User => User.id == id);
        }

        public bool isExistsByLogin(string login)
        {
            return context.UsersDbSet.Any(User => User.login == login);
        }

        public bool isExistsByMail(string mail)
        {
            return context.UsersDbSet.Any(User => User.email == mail);
        }


        public void Update(User item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
