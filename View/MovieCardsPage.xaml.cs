using MovieFinder.Models;
using MovieFinder.UserControls;
using MovieFinder.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для MovieCardsPage.xaml
    /// </summary>
    public partial class MovieCardsPage : Page, INotifyPropertyChanged
    {
        private ContentControl frameContentControl;
        private object previousContent;

        public MovieCardsPage(MovieViewModel movieViewModel)
        {
            InitializeComponent();
            frameContentControl = new ContentControl();
            DataContext = movieViewModel;
            paginationControl.Visibility = Visibility.Hidden;
        }

        private void cardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = DataContext as MovieViewModel;

            Movie model = context.SelectedMovieModel;

            if (model != null)
            {
                ////previousContent = Application.Current.MainWindow.Content;
                //var page = new FilmDiscriptionPage(model);
                //context.SelectedMovieModel = null;
                //page.btnBack.Click += BtnBack_Click;
                ////frameContentControl.Content = previousContent;
                //var mainWindow = Application.Current.MainWindow as MainWindow;
                //(Application.Current.MainWindow as MainWindow).navigationFrame.Content = page;


                var mainDataContext = App.Current.MainWindow.DataContext as MainWindowViewModel;
                mainDataContext.NavigateToMovieDescription.Execute(model);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ////App.Current.MainWindow.Content = previousContent;
            //var mainWindow = Application.Current.MainWindow as MainWindow;
            //mainWindow.navigationFrame.Content = previousContent;

            ((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel).NavigateToMovies.Execute(null);

        }

        private void cardListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var dataContext = DataContext as MovieViewModel;

            if (dataContext.isSearching == true)
            {
                return;
            }
            var scrollViewer = e.OriginalSource as ScrollViewer;

            if (scrollViewer != null && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                paginationControl.Visibility = Visibility.Visible;
            }
            else
            {
                paginationControl.Visibility = Visibility.Hidden;
            }
        }
    }
}

