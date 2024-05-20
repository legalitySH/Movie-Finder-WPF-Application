using MovieFinder.Database;
using MovieFinder.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MovieFinder.UserControls
{
    /// <summary>
    /// Логика взаимодействия для HistorySegment.xaml
    /// </summary>
    public partial class HistorySegment : UserControl
    {
        private UnitOfWorkContent unit;
        public HistorySegment()
        {
            InitializeComponent();
            DataContextChanged += HistorySegment_DataContextChanged;
            unit = new UnitOfWorkContent();
        }

        private void HistorySegment_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = DataContext as HistoryModel;

            if (dataContext == null) return;

            switch (dataContext.type)
            {
                case "movie":
                    {
                        var movie = unit.Movies.Get(dataContext.production_id);
                        var uri = new Uri(movie.image_url);
                        BitmapImage bitmapImage = new BitmapImage(uri);
                        Image.Source = bitmapImage;

                        Title.Text = movie.russian_name + $"({movie.year})";
                        Genres.Text = movie.genres;
                        Rating.Text = movie.rating.ToString();
                        Descrition.Text = movie.short_description;

                        break;
                    }
                case "serial":
                    {
                        var serial = unit.Serials.Get(dataContext.production_id);
                        var uri = new Uri(serial.image_url, UriKind.Absolute);
                        BitmapImage bitmapImage = new BitmapImage(uri);
                        Image.Source = bitmapImage;

                        Title.Text = serial.title + $"({serial.year})";
                        Genres.Text = serial.genres;
                        Rating.Text = serial.rating.ToString();
                        Descrition.Text = serial.short_description;
                        break;
                    }
            }
        }
    }
}
