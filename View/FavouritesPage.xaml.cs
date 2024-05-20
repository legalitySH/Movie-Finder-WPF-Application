using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для FavouritesPage.xaml
    /// </summary>
    public partial class FavouritesPage : Page
    {


        public FavouritesPage(FavouriteViewModel favouriteViewModel)
        {
            InitializeComponent();
            favouriteViewModel.UpdateData();
            DataContext = favouriteViewModel;
        }

        private void cardListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void cardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = DataContext as FavouriteViewModel;
            UnitOfWorkContent unitOfWorkContent = new UnitOfWorkContent();

            FavouriteModel model = context.SelectedFavourite;

            if (model != null)
            {
                switch (model.type)
                {
                    case "movie":
                        {
                            var movie = unitOfWorkContent.Movies.Get(model.production_id);
                            var mainDataContext = App.Current.MainWindow.DataContext as MainWindowViewModel;
                            mainDataContext.NavigateToMovieDescription.Execute(movie);
                            break;
                        }
                    case "serial":
                        {
                            var serial = unitOfWorkContent.Serials.Get(model.production_id) as Serial;
                            var mainDataContext = App.Current.MainWindow.DataContext as MainWindowViewModel;
                            mainDataContext.NavigateToSerialDescription.Execute(serial);
                            break;
                        }
                }
            }




        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel).NavigateToMovies.Execute(null);
        }
    }
}
