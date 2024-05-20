using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder.Database.Repositories
{
    public class BannedRepository : IRepository<int, BannedUser>
    {
        private ApplicationDbContext context;

        public BannedRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(BannedUser item)
        {
            context.BannedDbSet.Add(item);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var bannedUser = context.BannedDbSet.FirstOrDefault(ban => ban.id == id);
            if (bannedUser != null)
            {
                context.BannedDbSet.Remove(bannedUser);
                context.SaveChanges();
            }
        }


        public BannedUser? Get(int id)
        {
            return context.BannedDbSet.Find(id);
        }

        public BannedUser? GetByUserId(int userId)
        {
            return context.BannedDbSet.FirstOrDefault(banned => banned.user_id == userId);
        }

        public List<BannedUser> GetAll()
        {
            return context.BannedDbSet.ToList();
        }
        

        public bool isExists(int id)
        {
            return context.BannedDbSet.Any(ban => ban.id == id);
        }


        public void Update(BannedUser item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }

}