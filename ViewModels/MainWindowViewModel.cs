using GalaSoft.MvvmLight.Command;
using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.View;
using MovieFinder.View.Admin.Add;
using MovieFinder.View.Admin.UserManagment;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications.Messages;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;


namespace MovieFinder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Pages Fields

        public MovieCardsPage movieCardsPage { get; set; }
        public SerialsCardPage serialsCardPage { get; set; }
        public HistoryPage historyPage { get; set; }
        public FavouritesPage favouritesPage { get; set; }

        #endregion

        #region Properties
        private static User _authUser;
        public static User AuthorizedUser
        {
            get
            {
                return _authUser;
            }
            set
            {
                _authUser = value;
            }
        }

        public User AuthorizedUserNonStatic { get; set; }


        public UnitOfWorkContent UnitOfWorkContent;
        private string currentTheme;

        private Page _currentPage;

        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _prevPage = _currentPage;
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PreviousPage));
            }
        }

        private Page _prevPage;

        public Page PreviousPage
        {
            get
            {
                if (_prevPage == null) return new Page();

                return _prevPage;
            }
        }
        #endregion

        #region View Models

        public MovieViewModel MovieViewModel { get; set; }
        public SerialsViewModel SerialsViewModel { get; set; }
        public UsersViewModel UsersViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public FavouriteViewModel FavouriteViewModel { get; set; }

        #endregion

        #region Commands

        public ICommand ChangeThemeCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }
        public ICommand NavigateToMovies { get; set; }
        public ICommand NavigateToSerials { get; set; }
        public ICommand NavigateToHistory { get; set; }
        public ICommand NavigateToFavourites { get; set; }
        public ICommand NavigateToMovieDescription { get; set; }
        public ICommand NavigateToPreviousPage { get; set; }
        public ICommand NavigateToSerialDescription { get; set; }
        public ICommand NavigatoToUserManagment { get; set; }
        public ICommand NavigateToAddMovie { get; set; }
        public ICommand NavigateToAddSerial { get; set; }

        #endregion




        public MainWindowViewModel()
        {
            UnitOfWorkContent = new UnitOfWorkContent();

            MovieViewModel = new MovieViewModel(UnitOfWorkContent);
            SerialsViewModel = new SerialsViewModel(UnitOfWorkContent);
            UsersViewModel = new UsersViewModel(UnitOfWorkContent);
            HistoryViewModel = new HistoryViewModel(UnitOfWorkContent);
            FavouriteViewModel = new FavouriteViewModel(UnitOfWorkContent);




            NavigateToMovies = new RelayCommand(toMovies);
            NavigateToSerials = new RelayCommand(toSerials);
            NavigateToHistory = new RelayCommand(toHistory);
            NavigateToFavourites = new RelayCommand(ToFavourites);
            NavigateToPreviousPage = new RelayCommand(ToPreviousPage);
            NavigateToMovieDescription = new RelayCommand<Movie>(toMovieDescription);
            NavigateToSerialDescription = new RelayCommand<Serial>(toSerialDescription);
            NavigatoToUserManagment = new RelayCommand(toUserManagment);
            NavigateToAddMovie = new RelayCommand(toAddMovie);
            NavigateToAddSerial = new RelayCommand(toAddSerial);


            movieCardsPage = new MovieCardsPage(MovieViewModel);
            serialsCardPage = new SerialsCardPage(SerialsViewModel);
            historyPage = new HistoryPage(HistoryViewModel);
            favouritesPage = new FavouritesPage(FavouriteViewModel);


            currentTheme = "Dark";
            movieCardsPage.paginationControl.pageCount = MovieViewModel.countOfMoviePages;
            serialsCardPage.paginationControl.pageCount = SerialsViewModel.countOfSerialsPages;
            CurrentPage = movieCardsPage;
        }

        #region Command Methods

        public void toAddSerial()
        {
            AddSerial addSerialPage = new AddSerial();
            addSerialPage.ShowDialog();
        }

        public void toAddMovie()
        {
            AddMovie addMoviePage = new AddMovie();
            addMoviePage.ShowDialog();

        }

        public void toUserManagment()
        {
            UserManagment userManagmentWindow = new UserManagment();
            userManagmentWindow.ShowDialog();
        }

        public void ToPreviousPage()
        {
            CurrentPage = PreviousPage;
        }

        public void toSerialDescription(Serial serial)
        {
            SerialDescriptionPage serialDescriptionPage = new SerialDescriptionPage(serial);
            serialDescriptionPage.btnBack.Command = NavigateToPreviousPage;
            CurrentPage = serialDescriptionPage;
        }

        public void toMovieDescription(Movie movie)
        {
            FilmDiscriptionPage movieDescriptionPage = new FilmDiscriptionPage(movie);
            movieDescriptionPage.btnBack.Command = NavigateToPreviousPage;
            CurrentPage = movieDescriptionPage;
        }

        public void toSerials()
        {
            SerialsViewModel.UpdateData();
            CurrentPage = serialsCardPage;
        }

        public void toMovies()
        {
            MovieViewModel.UpdateData();
            CurrentPage = movieCardsPage;
        }

        public void toHistory()
        {
            if (AuthorizedUser != null)
            {
                CurrentPage = historyPage;
            }
            else
            {
                App.Notifier.ShowError("Сначало авторизируйтесь");
            }
        }

        public void ToFavourites()
        {
            FavouriteViewModel.UpdateData();
            if (AuthorizedUser != null)
            {
                CurrentPage = favouritesPage;
            }
            else
            {
                App.Notifier.ShowError("Сначало авторизируйтесь");
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





    }
}
