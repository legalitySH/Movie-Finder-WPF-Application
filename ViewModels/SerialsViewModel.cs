using DevExpress.Mvvm.Native;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualBasic.Devices;
using MovieFinder.Attributes;
using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.View.Admin.Edit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Messages;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;


namespace MovieFinder.ViewModels
{
    public class SerialsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Collections

        public ObservableCollection<Serial> SerialList { get; set; }
        public ObservableCollection<Serial> totalSerials { get; set; }
        public ObservableCollection<Serial> serialsFromDb { get; set; }


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

        #endregion

        #region Properties

        public bool IsPaginationVisible { get; set; }
        public bool isSearching { get; set; }
        public bool IsAdminMode
        {
            get
            {
                return UsersViewModel.IsAdminMode;
            }
        }
        public Serial? SelectedSerialModel { get; set; }

        #endregion

        #region Fields

        private UnitOfWorkContent UnitOfWorkContent;
        private const int COUNT_SERIALS_ON_PAGE = 40;
        public int countOfSerialsPages;
        private bool isFilterDrop;

        #endregion

        #region Commands

        public ICommand? AddSerialCommand { get; set; }
        public ICommand? RemovSerialCommand { get; set; }
        public ICommand ApplyFilterCommand { get; set; }
        public ICommand? EditSerialCommand { get; set; }
        public ICommand? SearchSerialCommand { get; set; }
        public ICommand? ChangePageContent { get; set; }

        public ICommand? AllMoviesCommand { get; set; }
        public ICommand? ToFavouritesCommand { get; set; }
        public ICommand? ToHistoryCommand { get; set; }

        #endregion


        public SerialsViewModel(UnitOfWorkContent UnitOfWorkContent)
        {
            this.UnitOfWorkContent = UnitOfWorkContent;
            totalSerials = SerialList = serialsFromDb = new ObservableCollection<Serial>(UnitOfWorkContent.Serials.GetAll());


            if (MainWindowViewModel.AuthorizedUser != null)
            {
                GetUserRecommendations();
            }
            else
            {
                totalSerials = new ObservableCollection<Serial>(UnitOfWorkContent.Serials.GetAll());
            }

            RatingSort = new ObservableCollection<string>(new List<string>() { "Более популярные", "Менее популярные" });
            Genres = new() { "Все жанры" };
            Years = new() { "Все года" };
            Countries = new() { "Все страны" };
            countOfSerialsPages = (totalSerials.Count / COUNT_SERIALS_ON_PAGE) + 1;


            ChangePageContent = new RelayCommand<string>(GetPageContent);
            SearchSerialCommand = new RelayCommand<string>(SearchSerials);
            AllMoviesCommand = new RelayCommand(DropFilters);
            ToFavouritesCommand = new RelayCommand(NavigateToFavourites);
            ToHistoryCommand = new RelayCommand(NavigateToHistory);
            ApplyFilterCommand = new RelayCommand(GetMoviesByFilter);



            RemovSerialCommand = new RelayCommand<Serial>(DeleteSerial);
            EditSerialCommand = new RelayCommand<Serial>(EditSerial);

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

        private void SearchSerials(string query)
        {
            IsPaginationVisible = false;
            isSearching = true;
            if (query == string.Empty)
            {
                SerialList = serialsFromDb;
                isSearching = false;
            }
            else
            {
                SerialList = MovieFinderSearcher.GetResultOfSerialSearch(SerialList, query);

            }
        }

        public void UpdateData()
        {
            totalSerials = SerialList = serialsFromDb = new ObservableCollection<Serial>(UnitOfWorkContent.Serials.GetAll());


            if (MainWindowViewModel.AuthorizedUser != null)
            {
                GetUserRecommendations();
            }
            else
            {
                totalSerials = new ObservableCollection<Serial>(UnitOfWorkContent.Serials.GetAll());
            }

            RatingSort = new ObservableCollection<string>(new List<string>() { "Более популярные", "Менее популярные" });
            Genres = new() { "Все жанры" };
            Years = new() { "Все года" };
            Countries = new() { "Все страны" };
            countOfSerialsPages = (totalSerials.Count / COUNT_SERIALS_ON_PAGE) + 1;

            GetPageContent("1");
            GetFilterParams();
        }

        #region Admin

        public void DeleteSerial(Serial serial)
        {
            Serial? serialFromDb = UnitOfWorkContent.Serials.GetByTitle(serial.title);
            if (serialFromDb != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить сериал?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    totalSerials.Remove(serial);
                    SerialList.Remove(serial);
                    serialsFromDb.Remove(serial);

                    DeleteSerialActivity(serial);
                    UnitOfWorkContent.Serials.Delete(serialFromDb.id ?? 0);
                    totalSerials.Remove(serial); SerialList.Remove(serial); serialsFromDb.Remove(serial);
                    App.Notifier.ShowSuccess("Сериал успешно удалён из базы данных");
                }


            }
        }

        public void DeleteSerialActivity(Serial serial)
        {
            UnitOfWorkContent.History.DeleteAllByTypeAndProductionId("serial", serial.id ?? 0);
            UnitOfWorkContent.Favourites.DeleteAllByTypeAndProductionId("serial", serial.id ?? 0);
            UnitOfWorkContent.Reviews.DeleteAllByTypeAndProductionId("serial", serial.id ?? 0);

            UnitOfWorkContent.DisableVoteTriger();
            UnitOfWorkContent.Votes.DeleteAllByTypeAndProductionId("serial", serial.id ?? 0);
            UnitOfWorkContent.EnableVoteTrigger();
        }

        public void EditSerial(Serial serial)
        {
            var editedSerial = SerialList.FirstOrDefault(s => s.id == serial.id);
            EditSerial page = new EditSerial(editedSerial);
            page.ShowDialog();
        }

        #endregion

        #region Filtration

        public void GetFilterParams()
        {
            foreach (var serial in totalSerials)
            {
                string[] genres = serial.genres.Split(' ');

                foreach (var genre in genres)
                {
                    if (!Genres.Contains(genre.ToLower()) && genre.ToLower() != string.Empty)
                    {
                        Genres.Add(genre.ToLower());
                    }
                }
                if (!Years.Contains(serial.year.ToString()))
                {
                    Years.Add(serial.year.ToString());
                }

                string[] countries = serial.countries.Split(' ');
                foreach (var country in countries)
                {
                    if (!Countries.Contains(country) && country != string.Empty)
                    {
                        Countries.Add(country);
                    }
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

            if (MainWindowViewModel.AuthorizedUser == null)
            {
                totalSerials = serialsFromDb;
            }
            else
            {
                GetUserRecommendations();
            }

            UpdatePaginationControl();
        }

        private void GetMoviesByFilter()
        {
            if (isFilterDrop)
            {
                return;
            }

            List<Serial>? filtredList = serialsFromDb.ToList();

            if (SelectedGenre != null && SelectedGenre != "Все жанры")
            {
                filtredList = filtredList.Where(movie => movie.genres.ToLower().Contains(SelectedGenre)).ToList();
            }

            if (SelectedCountry != null && SelectedCountry != "Все страны")
            {
                filtredList = filtredList.Where(serial => serial.countries.Contains(SelectedCountry)).ToList();
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

            totalSerials = filtredList.ToObservableCollection();
            UpdatePaginationControl();
        }

        #endregion

        #region Pagination

        public void GetPageContent(string page)
        {

            int pageInt = Convert.ToInt32(page);
            int endIndex = 0;
            int startIndex = (pageInt - 1) * COUNT_SERIALS_ON_PAGE;
            if (pageInt * COUNT_SERIALS_ON_PAGE - 1 > totalSerials.Count)
            {
                endIndex = totalSerials.Count;
            }
            else
            {
                endIndex = pageInt * COUNT_SERIALS_ON_PAGE - 1;
            }



            ObservableCollection<Serial> newSerials = new ObservableCollection<Serial>();

            for (int i = 0; i < totalSerials.Count; i++)
            {
                if (i >= startIndex && i <= endIndex)
                {
                    newSerials.Add(totalSerials[i]);
                }
            }


            SerialList = newSerials;
        }

        public void UpdatePaginationControl()
        {
            countOfSerialsPages = totalSerials.Count() / COUNT_SERIALS_ON_PAGE;
            var viewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;

            if (viewModel != null)
            {
                viewModel.serialsCardPage.paginationControl.pageCount = countOfSerialsPages;
                viewModel.serialsCardPage.paginationControl.currentPage.Text = 1.ToString();
                viewModel.serialsCardPage.paginationControl.rightButton.IsEnabled = true;
            }
            GetPageContent("1");
        }

        #endregion

        #region System recommendation

        private void GetUserRecommendations()
        {
            List<HistoryModel> serialHistories = UnitOfWorkContent.History.GetSerialsHistory(MainWindowViewModel.AuthorizedUser.id);
            List<Serial> serialsFromHistory = new List<Serial>();
            foreach (HistoryModel history in serialHistories)
            {
                var serial = UnitOfWorkContent.Serials.Get(history.production_id);
                if (serial != null)
                {
                    serialsFromHistory.Add(serial);
                }
            }

            List<FavouriteModel> serialFavourites = UnitOfWorkContent.Favourites.GetMovieFavourites(MainWindowViewModel.AuthorizedUser.id);
            List<Serial> serialFromFavourites = new List<Serial>();
            foreach (FavouriteModel favourite in serialFavourites)
            {
                var serial = UnitOfWorkContent.Serials.Get(favourite.production_id);
                if (serial != null)
                {
                    serialFromFavourites.Add(serial);
                }
            }

            List<Serial> userSerials = serialsFromHistory.Union(serialFromFavourites).ToList();

            string? mostPopularGenre = userSerials
                .GroupBy(movie => movie.genres)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            int? mostRecentYear = userSerials
                .OrderByDescending(movie => movie.year)
                .Select(movie => movie.year)
                .FirstOrDefault();

            string? mostPopularCountry = userSerials
                .GroupBy(movie => movie.countries)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            List<Serial> recommendedByGenre = totalSerials.Where(serial => serial.genres == mostPopularGenre).ToList();

            List<Serial> recommendedByYear = totalSerials.Where(serial => serial.year == mostRecentYear).ToList();


            List<Serial> recommendedMovies =
                recommendedByGenre
            .Concat(recommendedByYear)
            .GroupBy(serial => serial.id)
            .Select(group => group.First())
            .ToList();

            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(recommendedMovies));


            SerialList = recommendedMovies
                .Concat(totalSerials)
                .GroupBy(movie => movie.id)
                .Select(group => group.First())
                .ToObservableCollection();

            totalSerials = recommendedMovies
                .Concat(serialsFromDb)
                .GroupBy(movie => movie.id)
                .Select(group => group.First())
                .ToObservableCollection();

            totalSerials = totalSerials.OrderByDescending(movie => movie.rating).ToObservableCollection();
        }

        #endregion

    }
}
