using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToastNotifications.Messages;


namespace MovieFinder
{
    /// <summary>
    /// Логика взаимодействия для Filmxaml
    /// </summary>
    public partial class FilmDiscriptionPage : Page, INotifyPropertyChanged
    {
        private UnitOfWorkContent unitOfWorkContent;
        private FavouriteModel favourite;

        public FilmDiscriptionPage(Movie model)
        {

            InitializeComponent();
            DataContext = model;
            unitOfWorkContent = new UnitOfWorkContent();

            ReviewsControl.DataContext = model;
            var uri = new Uri(model.image_url);
            BitmapImage bitmapImage = new BitmapImage(uri);
            ImgUri.Source = bitmapImage;
            Title.Text = $"{model.russian_name}({model.year})";
            ShortDiscription.Text = model.short_description;
            AgeLimit.Text = model.age_limit.ToString() + "+";
            YearOfIssue.Text += model.year.ToString();
            Genres.Text += model.genres;
            Director.Text += model.director;
            Duration.Text += model.duration;
            longDescription.Text = model.description;
            Country.Text += model.countries;

            ratingControl.Visibility = Visibility.Hidden;
            Rating.Text = UpdateAndReturnRatingValue().ToString();

            var mainWindow = Application.Current.MainWindow as MainWindow;

            var dataContext = DataContext as Movie;

            if (MainWindowViewModel.AuthorizedUser != null)
            {
                ratingControl.Visibility = Visibility.Visible;

            }
            if (dataContext != null && unitOfWorkContent.Votes.GetAllByTypeAndProductionId("movie", dataContext.id).Count != 0)
            {

                var votes = unitOfWorkContent.Votes.GetAllByTypeAndProductionId("movie", model.id);
                int countOfVotes = 0;
                double totalRating = 0;

                Movie? modelFromDb = unitOfWorkContent.Movies.Get(model.id);

                if (modelFromDb != null)
                {
                    countOfVotes = votes.Count;
                }

                foreach (var vote in votes)
                {
                    totalRating += vote.rating_value;
                }

                double newRating = totalRating / Convert.ToDouble(countOfVotes);
                double roundedNumber = Math.Round(newRating, 2);

                Rating.Text = roundedNumber.ToString();
                ratingControl.ratingBar.Value = Convert.ToInt32(roundedNumber);
            }


            if (MainWindowViewModel.AuthorizedUser != null)
            {
                favourite = new FavouriteModel()
                {
                    production_id = model.id,
                    type = "movie",
                    user_id = MainWindowViewModel.AuthorizedUser.id
                };

                HistoryModel historyModel = new HistoryModel()
                {
                    production_id = model.id,
                    type = "movie",
                    user_id = MainWindowViewModel.AuthorizedUser.id,
                };

                if (!unitOfWorkContent.History.isExistsTotal(historyModel.production_id, historyModel.type, historyModel.user_id))
                {
                    unitOfWorkContent.History.Add(historyModel);
                }

                if (unitOfWorkContent.Favourites.isExistsTotal(favourite.production_id, favourite.type, favourite.user_id))
                {
                    Heart.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                }


                ratingControl.Visibility = Visibility.Visible;
                ratingControl.ratingBar.ValueChanged += RatingBar_ValueChanged;


            }
        }


        private void RatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var dataContext = DataContext as Movie;
            if (dataContext != null)
            {
                if (!unitOfWorkContent.Votes.isExistsTotal(dataContext.id, "movie", MainWindowViewModel.AuthorizedUser.id))
                {
                    var newVote = new VoteModel()
                    {
                        production_id = dataContext.id,
                        type = "movie",
                        user_id = MainWindowViewModel.AuthorizedUser.id,
                        rating_value = Convert.ToInt32(ratingControl.ratingBar.Value)
                    };

                    unitOfWorkContent.Votes.Add(newVote);
                    Movie? modelFromDb = unitOfWorkContent.Movies.Get(dataContext.id);
                    double newRating = UpdateAndReturnRatingValue();

                    if (modelFromDb != null)
                    {
                        modelFromDb.rating = newRating;
                        modelFromDb.votes = ++modelFromDb.votes;
                        unitOfWorkContent.Movies.Update(modelFromDb);
                    }
                    UpdateAndReturnRatingValue();
                }
                else
                {
                    var userVote = unitOfWorkContent.Votes.GetUserVote("movie", dataContext.id, MainWindowViewModel.AuthorizedUser.id);
                    int newRatingToUpdate = Convert.ToInt32(ratingControl.ratingBar.Value);
                    if (userVote != null)
                    {
                        userVote.rating_value = newRatingToUpdate;
                        unitOfWorkContent.Votes.Update(userVote);
                    }
                    Movie? modelFromDb = unitOfWorkContent.Movies.Get(dataContext.id);

                    double newRating = UpdateAndReturnRatingValue();

                    if (modelFromDb != null)
                    {
                        modelFromDb.rating = newRating;
                        unitOfWorkContent.Movies.Update(modelFromDb);
                    }
                    UpdateAndReturnRatingValue();
                }
            }
        }

        private void Button_ToFavourite_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                var dataContext = DataContext as Movie;
                favourite = new FavouriteModel()
                {
                    production_id = dataContext.id,
                    type = "movie",
                    user_id = MainWindowViewModel.AuthorizedUser.id
                };
                if (unitOfWorkContent.Favourites.isExistsTotal(favourite.production_id, favourite.type, favourite.user_id))
                {
                    Heart.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

                    var mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
                    if (mainWindowViewModel != null)
                    {
                        var favouriteFromList = mainWindowViewModel.FavouriteViewModel.FavouriteList
                        .FirstOrDefault(fav => fav.production_id == favourite.production_id && fav.type == favourite.type && favourite.user_id == MainWindowViewModel.AuthorizedUser.id);

                        if (favouriteFromList != null)
                        {
                            mainWindowViewModel.FavouriteViewModel.FavouriteList.Remove(favouriteFromList);
                        }

                    }

                    unitOfWorkContent.Favourites.DeleteByFavourite(favourite);
                    App.Notifier.ShowSuccess("Фильм успешно удалён из избранного");
                }
                else
                {
                    Heart.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                    var mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
                    if (mainWindowViewModel != null)
                    {
                        mainWindowViewModel.FavouriteViewModel.FavouriteList.Add(favourite);
                    }
                    unitOfWorkContent.Favourites.Add(favourite);
                    App.Notifier.ShowSuccess("Фильм успешно добавлен в избранное");
                }

            }
            else
            {
                App.Notifier.ShowError("Чтобы добавить фильм в избранное - авторизируйтесь в аккаунт");

            }



        }

        private double UpdateAndReturnRatingValue()
        {
            var dataContext = DataContext as Movie;
            if (dataContext != null && unitOfWorkContent.Votes.GetAllByTypeAndProductionId("movie", dataContext.id).Count != 0)
            {
                var votes = unitOfWorkContent.Votes.GetAllByTypeAndProductionId("movie", dataContext.id);
                int countOfVotes = 0;
                double totalRating = 0;

                Movie? modelFromDb = unitOfWorkContent.Movies.Get(dataContext.id);

                if (modelFromDb != null)
                {
                    countOfVotes = votes.Count;
                }

                foreach (var vote in votes)
                {
                    totalRating += vote.rating_value;
                }

                double newRating = totalRating / Convert.ToDouble(countOfVotes);
                double roundedNumber = Math.Round(newRating, 2);

                Rating.Text = roundedNumber.ToString();
                return roundedNumber;
            }

            ratingControl.ratingBar.Value = 0;

            return 0;
        }

    }
}


