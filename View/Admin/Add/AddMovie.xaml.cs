using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications.Messages;

namespace MovieFinder.View.Admin.Add
{
    /// <summary>
    /// Логика взаимодействия для AddMovie.xaml
    /// </summary>
    public partial class AddMovie : Window
    {


        private MainWindow mainWindow;

        bool isEdited = false;

        public AddMovie()
        {
            InitializeComponent();
            Url.TextChanged += Url_TextChanged;

            mainWindow = App.Current.MainWindow as MainWindow;
            App.Current.MainWindow = this;

            Closed += AddMovie_Closed;

            Uri uri = new Uri("https://domsveta24.ru/image/cache/catalog/image/cache/catalog/goods/OD_2443_1A/OD_2443_1A-1000x1000.webp");
            BitmapImage bitmapImage = new BitmapImage(uri); ;
            Image.Source = bitmapImage;

        }


        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            float rating = 0;
            string uri;
            int year = 0;
            int age = 0;
            int duration = 0;
            try
            {
                rating = Convert.ToSingle(Rating.Text);
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError("Не удалось получить рейтинг фильма");
                return;
            }
            try
            {
                Uri uriTest = new Uri(Url.Text, UriKind.Absolute);
                BitmapImage bitmapImage = new BitmapImage(uriTest);
                uri = Url.Text;

            }
            catch (Exception ex)
            {
                App.Notifier.ShowError("Не удалось установить изображение для фильма");
                uri = "https://domsveta24.ru/image/cache/catalog/image/cache/catalog/goods/OD_2443_1A/OD_2443_1A-1000x1000.webp";
                return;
            }

            try
            {
                year = Convert.ToInt32(Year.Text);
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError("Не удалось установить год выпуска для фильма");
                return;
            }

            try
            {
                age = Convert.ToInt32(AgeLimit.Text);
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError("Не удалось установить возрастное ограничение для фильма");
                return;
            }

            try
            {
                duration = Convert.ToInt32(Duration.Text);
            }
            catch (Exception ex)
            {
                App.Notifier.ShowError("Не удалось установить длительность для фильма");
                return;
            }

            Movie movie = new Movie()
            {
                russian_name = RussianName.Text,
                original_name = OriginalName.Text,
                image_url = uri,
                rating = rating,
                year = year,
                duration = duration,
                description = Description.Text,
                short_description = ShortDescription.Text,
                age_limit = age,
                countries = Countries.Text,
                genres = Genres.Text,
                director = Director.Text
            };
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = Validator.TryValidateObject(movie, new ValidationContext(movie), results, true);

            if (!isValid)
            {
                App.Notifier.ShowError(results[0].ErrorMessage);
            }
            else
            {
                isEdited = true;

                var unitOfWorkContent = new UnitOfWorkContent();

                unitOfWorkContent.Movies.Add(movie);

                var mainWindowDataContext = mainWindow.DataContext as MainWindowViewModel;
                App.UpdateView(mainWindow);
                mainWindowDataContext.MovieViewModel.UpdateData();
                App.UpdateView(mainWindow);
                
                Close();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddMovie_Closed(object? sender, EventArgs e)
        {
            App.Current.MainWindow = mainWindow;
            if (isEdited)
            {
                App.Notifier.ShowSuccess("Успешно изменено!");
            }
            Close();
        }

        private void Url_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Uri uri = new Uri(Url.Text, UriKind.Absolute);
                BitmapImage bitmapImage = new BitmapImage(uri);
                Image.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                Uri uri = new Uri("https://domsveta24.ru/image/cache/catalog/image/cache/catalog/goods/OD_2443_1A/OD_2443_1A-1000x1000.webp");
                BitmapImage bitmapImage = new BitmapImage(uri); ;
                Image.Source = bitmapImage;
                return;
            }
        }



        
    }
}
