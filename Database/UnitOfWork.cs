using MovieFinder.Database.Repositories;

namespace MovieFinder.Database
{
    public class UnitOfWorkContent : IDisposable
    {
        private ApplicationDbContext context;

        private MovieRepository? movieRepository;
        private SerialsRepository? serialsRepository;
        private UsersRepository? usersRepository;
        private HistoryRepository? historyRepository;
        private FavouriteRepository? favouriteRepository;
        private ReviewRepository? reviewRepository;
        private VotesRepository? votesRepository;
        private BannedRepository? bannedRepository;


        public UnitOfWorkContent()
        {
            context = new ApplicationDbContext();
        }

        public MovieRepository Movies
        {
            get
            {
                if (movieRepository == null) movieRepository = new MovieRepository(context);
                return movieRepository;
            }
        }

        public SerialsRepository Serials
        {
            get
            {
                if (serialsRepository == null) serialsRepository = new SerialsRepository(context);
                return serialsRepository;
            }
        }

        public UsersRepository Users
        {
            get
            {
                if (usersRepository == null) usersRepository = new UsersRepository(context);
                return usersRepository;
            }
        }

        public HistoryRepository History
        {
            get
            {
                if (historyRepository == null) historyRepository = new HistoryRepository(context);
                return historyRepository;
            }
        }

        public FavouriteRepository Favourites
        {
            get
            {
                if (favouriteRepository == null) favouriteRepository = new FavouriteRepository(context);
                return favouriteRepository;
            }
        }

        public ReviewRepository Reviews
        {
            get
            {
                if (reviewRepository == null) reviewRepository = new ReviewRepository(context);
                return reviewRepository;
            }
        }

        public VotesRepository Votes
        {
            get
            {
                if (votesRepository == null) votesRepository = new VotesRepository(context);
                return votesRepository;
            }
        }

        public BannedRepository Banned
        {
            get
            {
                if(bannedRepository == null) bannedRepository = new BannedRepository(context);
                return bannedRepository;
            }
        }

        public void EnableVoteTrigger()
        {
            context.DeleteMovieOrSerialTriggerEnable();
        }

        public void DisableVoteTriger()
        {
            context.DeleteMovieOrSerialTriggerDisable();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
