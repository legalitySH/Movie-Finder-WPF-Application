using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToastNotifications.Messages;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для SerialDescriptionPage.xaml
    /// </summary>
    public partial class SerialDescriptionPage : Page
    {
        private UnitOfWorkContent unitOfWorkContent;
        private FavouriteModel favourite;


        public SerialDescriptionPage(Serial model)
        {
            InitializeComponent();
            unitOfWorkContent = new UnitOfWorkContent();
            DataContext = model;

            var uri = new Uri(model.image_url);
            BitmapImage bitmapImage = new BitmapImage(uri);
            ImgUri.Source = bitmapImage;
            Title.Text = $"{model.title}({model.year})";
            ShortDiscription.Text = model.short_description;
            AgeLimit.Text = model.age_limit.ToString() + "+";
            YearOfIssue.Text += model.year.ToString();
            Genres.Text += model.genres;
            Country.Text += model.countries;
            Director.Text += model.director;
            longDescription.Text = model.description;

            ratingControl.Visibility = Visibility.Hidden;
            Rating.Text = UpdateAndReturnRatingValue().ToString();

            ReviewsControl.DataContext = model;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var dataContextOfMain = mainWindow.DataContext as MainWindowViewModel;

                mainWindow.navigationFrame.Visibility = Visibility.Visible;
            }




            if (MainWindowViewModel.AuthorizedUser != null)
            {
                HistoryModel historyModel = new HistoryModel()
                {
                    production_id = model.id ?? 0,
                    type = "serial",
                    user_id = MainWindowViewModel.AuthorizedUser.id,
                };

                favourite = new FavouriteModel()
                {
                    production_id = model.id ?? 0,
                    type = "serial",
                    user_id = MainWindowViewModel.AuthorizedUser.id
                };

                if (!unitOfWorkContent.History.isExistsTotal(historyModel.production_id, historyModel.type, historyModel.user_id))
                {
                    unitOfWorkContent.History.Add(historyModel);
                }

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

            var dataContext = App.Current.MainWindow.DataContext as MainWindowViewModel;
            dataContext.HistoryViewModel.UpdateData();
            dataContext.FavouriteViewModel.UpdateData();
        }

        private void RatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var dataContext = DataContext as Serial;
            if (dataContext != null)
            {
                if (!unitOfWorkContent.Votes.isExistsTotal(dataContext.id ?? 0, "serial", MainWindowViewModel.AuthorizedUser.id))
                {
                    var newVote = new VoteModel()
                    {
                        production_id = dataContext.id ?? 0,
                        type = "serial",
                        user_id = MainWindowViewModel.AuthorizedUser.id,
                        rating_value = Convert.ToInt32(ratingControl.ratingBar.Value)
                    };

                    unitOfWorkContent.Votes.Add(newVote);
                    Serial? modelFromDb = unitOfWorkContent.Serials.Get(dataContext.id ?? 0);
                    double newRating = UpdateAndReturnRatingValue();

                    if (modelFromDb != null)
                    {
                        modelFromDb.rating = newRating;
                        modelFromDb.votes = ++modelFromDb.votes;
                        unitOfWorkContent.Serials.Update(modelFromDb);
                    }
                    UpdateAndReturnRatingValue();
                }
                else
                {
                    var userVote = unitOfWorkContent.Votes.GetUserVote("serial", dataContext.id ?? 0, MainWindowViewModel.AuthorizedUser.id);
                    int newRatingToUpdate = Convert.ToInt32(ratingControl.ratingBar.Value);
                    if (userVote != null)
                    {
                        userVote.rating_value = newRatingToUpdate;
                        unitOfWorkContent.Votes.Update(userVote);
                    }
                    Serial? modelFromDb = unitOfWorkContent.Serials.Get(dataContext.id ?? 0);

                    double newRating = UpdateAndReturnRatingValue();

                    if (modelFromDb != null)
                    {
                        modelFromDb.rating = newRating;
                        unitOfWorkContent.Serials.Update(modelFromDb);
                    }
                    UpdateAndReturnRatingValue();
                }
            }
        }


        private void Button_ToFavourite_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                var dataContext = DataContext as Serial;
                favourite = new FavouriteModel()
                {
                    production_id = dataContext.id ?? 0,
                    type = "serial",
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
                    App.Notifier.ShowSuccess("Сериал успешно удалён из избранного");

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
                    App.Notifier.ShowSuccess("Сериал успешно добавлен в избранное");

                }

            }
            else
            {
                App.Notifier.ShowError("Чтобы добавить сериал в избранное - авторизируйтесь в аккаунт");
            }
        }



        private double UpdateAndReturnRatingValue()
        {
            var dataContext = DataContext as Serial;
            if (dataContext != null && unitOfWorkContent.Votes.GetAllByTypeAndProductionId("serial", dataContext.id ?? 0).Count != 0)
            {
                var votes = unitOfWorkContent.Votes.GetAllByTypeAndProductionId("serial", dataContext.id ?? 0);
                int countOfVotes = 0;
                double totalRating = 0;

                Serial? modelFromDb = unitOfWorkContent.Serials.Get(dataContext.id ?? 0);

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
