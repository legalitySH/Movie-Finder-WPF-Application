using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;

namespace MovieFinder.Database.Repositories
{
    public class SerialsRepository : IRepository<int, Serial>
    {
        private ApplicationDbContext context;

        public SerialsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Serial item)
        {
            if (!isExists(item.id ?? int.MaxValue))
            {
                context.SerialsDbSet.Add(item);
                 context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var serial = context.SerialsDbSet.FirstOrDefault(serial => serial.id == id);
            if (serial != null)
            {
                context.SerialsDbSet.Remove(serial);
                context.SaveChanges();
            }
        }

        public Serial? Get(int id)
        {
            var serial = context.SerialsDbSet.FirstOrDefault(serial => serial.id == id);
            return serial;
        }

        public Serial? GetByTitle(string title)
        {
            return context.SerialsDbSet.FirstOrDefault(serial => serial.title == title);
        }

        public List<Serial> GetAll()
        {
            return context.SerialsDbSet.ToList();
        }

        public bool isExists(int id)
        {
            return context.SerialsDbSet.Any(serial => serial.id == id);
        }

        public void Update(Serial item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
