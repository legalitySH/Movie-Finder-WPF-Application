using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {

        private ContentControl frameContentControl;
        private object previousContent;


        public HistoryPage(HistoryViewModel historyViewModel)
        {
            InitializeComponent();
            frameContentControl = new ContentControl();
            historyViewModel.UpdateData();
            DataContext = historyViewModel;
        }

        private void cardListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void cardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = DataContext as HistoryViewModel;
            UnitOfWorkContent unitOfWorkContent = new UnitOfWorkContent();

            HistoryModel model = context.SelectedHistory;

            if (model != null)
            {
                switch (model.type)
                {
                    case "movie":
                        {
                            var movie = unitOfWorkContent.Movies.Get(model.production_id) as Movie;
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
            Application.Current.MainWindow.Content = frameContentControl.Content;
        }
    }
}
