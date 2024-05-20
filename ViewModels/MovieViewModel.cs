using DevExpress.Mvvm.Native;
using GalaSoft.MvvmLight.Command;
using MovieFinder.Attributes;
using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.View.Admin.Edit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Messages;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace MovieFinder.ViewModels
{

    public class MovieViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Fields and Propeties


        public ObservableCollection<Movie> MoviesList { get; set; }
        public ObservableCollection<Movie> totalMovies { get; set; }
        public ObservableCollection<Movie> moviesFromDb { get; set; }

        public Movie? SelectedMovieModel { get; set; }

        public ObservableCollection<string> Genres { get; set; }

        private string? _selectedGenre;
        public string? SelectedGenre
        {
            get { return _selectedGenre; }
            set { _selectedGenre = value; ApplyFilterCommand.Execute(null); }
        }

        public ObservableCollection<string> RatingSort { get; set; }

        private string? _selectedRating;
        public string? SelectedRating
        {
            get { return _selectedRating; }
            set { _selectedRating = value; ApplyFilterCommand.Execute(null); }
        }

        public ObservableCollection<string> Years { get; set; }
        private string? _selectedYear;
        public string? SelectedYear
        {
            get { return _selectedYear; }
            set { _selectedYear = value; ApplyFilterCommand.Execute(null); }
        }


        public ObservableCollection<string> Countries { get; set; }
        private string? _selectedCountry;
        public string? SelectedCountry
        {
            get { return _selectedCountry; }
            set { _selectedCountry = value; ApplyFilterCommand.Execute(null); }
        }

        private UnitOfWorkContent UnitOfWorkContent;
        public bool IsPaginationVisible { get; set; }
        public bool isSearching { get; set; }


        private const int COUNT_MOVIES_ON_PAGE = 40;
        public int countOfMoviePages;

        private bool isFilterDrop;

        public bool IsAdminMode
        {
            get
            {
                return UsersViewModel.IsAdminMode;
            }
        }


        #endregion

        #region Commands

        public ICommand? AddMovieCommand { get; set; }
        public ICommand? RemoveMovieCommand { get; set; }
        public ICommand ApplyFilterCommand { get; set; }
        public ICommand? EditMovieCommand { get; set; }
        public ICommand? SearchMovieCommand { get; set; }
        public ICommand? ChangePageContent { get; set; }
        public ICommand? AllMoviesCommand { get; set; }
        public ICommand? ToFavouritesCommand { get; set; }
        public ICommand? ToHistoryCommand { get; set; }

        #endregion


        public MovieViewModel(UnitOfWorkContent UnitOfWorkContent)
        {
            this.UnitOfWorkContent = UnitOfWorkContent;
            moviesFromDb = MoviesList = totalMovies = new ObservableCollection<Movie>(UnitOfWorkContent.Movies.GetAll());
            isFilterDrop = false;

            if (MainWindowViewModel.AuthorizedUser != null)
            {
                GetUserRecommendations();
            }
            else
            {
                MoviesList = new ObservableCollection<Movie>(UnitOfWorkContent.Movies.GetAll());
            }


            RatingSort = new ObservableCollection<string>(new List<string>() { "Более популярные", "Менее популярные" });
            Genres = new() { "Все жанры" };
            Years = new() { "Все года" };
            Countries = new() { "Все страны" };
            countOfMoviePages = (totalMovies.Count / COUNT_MOVIES_ON_PAGE) + 1;

            AllMoviesCommand = new RelayCommand(DropFilters);
            ApplyFilterCommand = new RelayCommand(GetMoviesByFilter);
            ChangePageContent = new RelayCommand<string>(GetPageContent);
            SearchMovieCommand = new RelayCommand<string>(SearchMovies);
            ToFavouritesCommand = new RelayCommand(NavigateToFavourites);
            ToHistoryCommand = new RelayCommand(NavigateToHistory);

            // AdminCommands 

            RemoveMovieCommand = new RelayCommand<Movie>(DeleteMovie);
            EditMovieCommand = new RelayCommand<Movie>(EditMovie);

            GetPageContent("1");
            GetFilterParams();

        }

        public void NavigateToHistory()
        {
            Window currentWindow = App.Current.MainWindow;
            var viewModel = currentWindow.DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.toHistory();
            }
        }

        private void NavigateToFavourites()
        {
            Window currentWindow = App.Current.MainWindow;
            var viewModel = currentWindow.DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.ToFavourites();
            }
        }

        private void SearchMovies(string query)
        {
            IsPaginationVisible = false;
            isSearching = true;
            if (query == string.Empty)
            {
                MoviesList = moviesFromDb;
                isSearching = false;
            }
            else
            {
                MoviesList = MovieFinderSearcher.GetResultOfMovieSearch(moviesFromDb, query);

            }
        }

        #region Admin

        public void DeleteMovie(Movie movie)
        {
            Movie? movieFromDb = UnitOfWorkContent.Movies.GetByTitle(movie.russian_name);
            if (movieFromDb != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить фильм?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    totalMovies.Remove(movie);
                    MoviesList.Remove(movie);
                    moviesFromDb.Remove(movie);

                    DeleteMovieActivity(movie);
                    UnitOfWorkContent.Movies.Delete(movieFromDb.id);
                    totalMovies.Remove(movie); MoviesList.Remove(movie); moviesFromDb.Remove(movie);
                    App.Notifier.ShowSuccess("Фильм успешно удалён из базы данных");
                }


            }
        }

        public void EditMovie(Movie movie)
        {
            var editedMovie = MoviesList.FirstOrDefault(m => m.id == movie.id);
            EditMovie page = new EditMovie(editedMovie);
            page.ShowDialog();
            
        }

        public void DeleteMovieActivity(Movie movie)
        {
            UnitOfWorkContent.History.DeleteAllByTypeAndProductionId("movie", movie.id);
            UnitOfWorkContent.Favourites.DeleteAllByTypeAndProductionId("movie", movie.id);
            UnitOfWorkContent.Reviews.DeleteAllByTypeAndProductionId("movie", movie.id);

            UnitOfWorkContent.DisableVoteTriger();
            UnitOfWorkContent.Votes.DeleteAllByTypeAndProductionId("movie", movie.id);
            UnitOfWorkContent.EnableVoteTrigger();
        }

        public void UpdateData()
        {
            moviesFromDb = MoviesList = totalMovies = new ObservableCollection<Movie>(UnitOfWorkContent.Movies.GetAll());
            isFilterDrop = false;

            if (MainWindowViewModel.AuthorizedUser != null)
            {
                GetUserRecommendations();
            }
            else
            {
                MoviesList = new ObservableCollection<Movie>(UnitOfWorkContent.Movies.GetAll());
            }


            RatingSort = new ObservableCollection<string>(new List<string>() { "Более популярные", "Менее популярные" });
            Genres = new() { "Все жанры" };
            Years = new() { "Все года" };
            Countries = new() { "Все страны" };
            countOfMoviePages = (totalMovies.Count / COUNT_MOVIES_ON_PAGE) + 1;

            GetPageContent("1");
            GetFilterParams();

            OnPropertyChanged(nameof(MoviesList));
            
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

        #region Filtration


        public void GetFilterParams()
        {
            foreach (var movie in totalMovies)
            {
                if (!Genres.Contains(movie.genres))
                {
                    Genres.Add(movie.genres);
                }
                if (!Years.Contains(movie.year.ToString()))
                {
                    Years.Add(movie.year.ToString());
                }
                if (!Countries.Contains(movie.countries))
                {
                    Countries.Add(movie.countries);
                }
            }
        }

        private void DropFilters()
        {
            isFilterDrop = true;

            SelectedGenre = Genres[0];
            SelectedRating = RatingSort[0];
            SelectedYear = Years[0];
            SelectedCountry = Countries[0];

            isFilterDrop = false;

            if (MainWindowViewModel.AuthorizedUser != null)
            {
                GetUserRecommendations();
            }
            else
            {
                totalMovies = moviesFromDb;
            }
            UpdatePaginationControl();
        }

        private void GetMoviesByFilter()
        {
            if (isFilterDrop)
            {
                return;
            }

            List<Movie> filtredList = moviesFromDb.ToList();

            if (SelectedGenre != null && SelectedGenre != "Все жанры")
            {
                filtredList = filtredList.Where(movie => movie.genres == SelectedGenre).ToList();
            }

            if (SelectedCountry != null && SelectedCountry != "Все страны")
            {
                filtredList = filtredList.Where(movie => movie.countries == SelectedCountry).ToList();
            }

            if (SelectedYear != null && SelectedYear != "Все года")
            {
                int selectedYear = int.Parse(SelectedYear);

                filtredList = filtredList.Where(movie => movie.year == selectedYear).ToList();
            }

            if (SelectedRating != null && SelectedRating != "Более популярные")
            {
                filtredList = filtredList.OrderBy(movie => movie.rating).ToList();
            }

            totalMovies = filtredList.ToObservableCollection();
            UpdatePaginationControl();
        }

        #endregion

        #region Pagination

        public void GetPageContent(string page)
        {

            int pageInt = Convert.ToInt32(page);
            int endIndex = 0;
            int startIndex = (pageInt - 1) * COUNT_MOVIES_ON_PAGE;
            if (pageInt * COUNT_MOVIES_ON_PAGE - 1 > totalMovies.Count)
            {
                endIndex = totalMovies.Count;
            }
            else
            {
                endIndex = pageInt * COUNT_MOVIES_ON_PAGE - 1;
            }



            ObservableCollection<Movie> newMovies = new ObservableCollection<Movie>();

            for (int i = 0; i < totalMovies.Count; i++)
            {
                if (i >= startIndex && i <= endIndex)
                {
                    newMovies.Add(totalMovies[i]);
                }
            }


            MoviesList = newMovies;

        }

        public void UpdatePaginationControl()
        {
            countOfMoviePages = totalMovies.Count() / COUNT_MOVIES_ON_PAGE;
            var viewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;

            if (viewModel != null)
            {
                viewModel.movieCardsPage.paginationControl.pageCount = countOfMoviePages;
                viewModel.movieCardsPage.paginationControl.currentPage.Text = 1.ToString();
                viewModel.movieCardsPage.paginationControl.rightButton.IsEnabled = true;
                viewModel.movieCardsPage.paginationControl.Visibility = Visibility.Visible;
            }
            GetPageContent("1");
        }

        #endregion

        #region System recommendation
        private void GetUserRecommendations()
        {
            List<HistoryModel> movieHistories = UnitOfWorkContent.History.GetMoviesHistory(MainWindowViewModel.AuthorizedUser.id);
            List<Movie> moviesFromHistory = new List<Movie>();
            foreach (HistoryModel history in movieHistories)
            {
                var movie = UnitOfWorkContent.Movies.Get(history.production_id);
                if (movie != null)
                {
                    moviesFromHistory.Add(movie);
                }
            }

            List<FavouriteModel> movieFavourites = UnitOfWorkContent.Favourites.GetMovieFavourites(MainWindowViewModel.AuthorizedUser.id);
            List<Movie> moviesFromFavourites = new List<Movie>();
            foreach (FavouriteModel favourite in movieFavourites)
            {
                var movie = UnitOfWorkContent.Movies.Get(favourite.production_id);
                if (movie != null)
                {
                    moviesFromFavourites.Add(movie);
                }
            }

            List<Movie> userMovies = moviesFromHistory.Union(moviesFromFavourites).ToList();

            string? mostPopularGenre = userMovies
                .GroupBy(movie => movie.genres)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            int? mostRecentYear = userMovies
                .OrderByDescending(movie => movie.year)
                .Select(movie => movie.year)
                .FirstOrDefault();

            string? mostPopularCountry = userMovies
                .GroupBy(movie => movie.countries)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            List<Movie> recommendedByGenre = totalMovies.Where(movie => movie.genres == mostPopularGenre).ToList();

            List<Movie> recommendedByYear = totalMovies.Where(movie => movie.year == mostRecentYear).ToList();


            List<Movie> recommendedMovies = recommendedByGenre
            .Concat(recommendedByYear)
            .GroupBy(movie => movie.id)
            .Select(group => group.First())
            .ToList();


            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(recommendedMovies));

            recommendedMovies.OrderByDescending(movie => movie.rating);


            MoviesList = recommendedMovies
                .Concat(totalMovies)
                .GroupBy(movie => movie.id)
                .Select(group => group.First())
                .ToObservableCollection();
            totalMovies = recommendedMovies
                .Concat(moviesFromDb)
                .GroupBy(movie => movie.id)
                .Select(group => group.First())
                .ToObservableCollection();

            totalMovies = totalMovies.OrderByDescending(movie => movie.rating).ToObservableCollection();
        }
        #endregion
    }



}


