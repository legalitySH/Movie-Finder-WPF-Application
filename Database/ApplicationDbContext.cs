using Microsoft.EntityFrameworkCore;
using MovieFinder.Models;
using System.Configuration;
using ToastNotifications.Messages;

namespace MovieFinder.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Movie> MoviesDbSet { get; set; }
        public DbSet<Serial> SerialsDbSet { get; set; }
        public DbSet<User> UsersDbSet { get; set; }
        public DbSet<HistoryModel> HistoryDbSet { get; set; }
        public DbSet<FavouriteModel> FavouritesDbSet { get; set; }
        public DbSet<Review> ReviewsDbSet { get; set; }
        public DbSet<VoteModel> VotesDbSet { get; set; }
        public DbSet<BannedUser> BannedDbSet {  get; set; }

        public ApplicationDbContext()
    : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["movieFinderConnectionString"].ConnectionString); // App.config
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>().ToTable("movies_table");

            modelBuilder.Entity<Serial>().ToTable("serials_table");

            modelBuilder.Entity<User>().ToTable("users");

            modelBuilder.Entity<HistoryModel>().ToTable("history");

            modelBuilder.Entity<FavouriteModel>().ToTable("favourites");

            modelBuilder.Entity<Review>().ToTable("reviews");

            modelBuilder.Entity<VoteModel>().ToTable("votes");

            modelBuilder.Entity<BannedUser>().ToTable("black_list");
        }


        public void DeleteMovieOrSerialTriggerEnable()
        {
            try
            {
                Database.ExecuteSqlRaw("ENABLE TRIGGER UpdateVoteCount ON votes");
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError(ex.Message);
            }

        }

        public void DeleteMovieOrSerialTriggerDisable()
        {
            try
            {
                Database.ExecuteSqlRaw("DISABLE TRIGGER UpdateVoteCount ON votes");
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError(ex.Message);
            }


        }




    }
}
