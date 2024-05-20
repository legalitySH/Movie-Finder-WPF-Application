using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using Syncfusion.Windows.Shared;
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

namespace MovieFinder.View.Admin.Edit
{
    /// <summary>
    /// Логика взаимодействия для EditSerial.xaml
    /// </summary>
    public partial class EditSerial : Window
    {
        private MainWindow mainWindow;

        bool isEdited = false;

        public EditSerial(Serial serial)
        {
            InitializeComponent();
            DataContext = serial;

            Title.Text = serial.title;
            Rating.Text = serial.rating.ToString();
            Year.Text = serial.year.ToString();
            Url.TextChanged += Url_TextChanged;
            Url.Text = serial.image_url;
            AgeLimit.Text = serial.age_limit.ToString();
            ShortDescription.Text = serial.short_description;
            Description.Text = serial.description;
            Director.Text = serial.director;
            Genres.Text = serial.genres;
            Countries.Text = serial.countries;


            mainWindow = App.Current.MainWindow as MainWindow;
            App.Current.MainWindow = this;

            Closed += EditSerial_Closed;

        }

        private void EditSerial_Closed(object? sender, EventArgs e)
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


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as Serial;
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


            Serial serial = new Serial()
            {
                id = dataContext.id,
                title = Title.Text,
                image_url = uri,
                rating = rating,
                year = year,
                description = Description.Text,
                short_description = ShortDescription.Text,
                age_limit = age,
                countries = Countries.Text,
                genres = Genres.Text,
                director = Director.Text,
            };

           
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = Validator.TryValidateObject(serial, new ValidationContext(serial), results, true);

            if (!isValid)
            {
                App.Notifier.ShowError(results[0].ErrorMessage);
            }
            else
            {
                isEdited = true;

                var unitOfWorkContent = new UnitOfWorkContent();

                var serialToEdit = unitOfWorkContent.Serials.Get(dataContext.id ?? 0);

                if (serialToEdit != null)
                {
                    var mainWindowDataContext = mainWindow.DataContext as MainWindowViewModel;

                    var serialInList = mainWindowDataContext.SerialsViewModel.SerialList.FirstOrDefault(s => s.id == serial.id);
                    if (serialInList != null)
                    {
                        serialInList.title = serial.title;
                        serialInList.image_url = serial.image_url;
                        serialInList.rating = serial.rating;
                        serialInList.year = serial.year;
                        serialInList.description = serial.description;
                        serialInList.short_description = serial.short_description;
                        serialInList.age_limit = serial.age_limit;
                        serialInList.countries = serial.countries;
                        serialInList.genres = serial.genres;
                        serialInList.director = serial.director;
                    }

                    var serialInTotalList = mainWindowDataContext.SerialsViewModel.totalSerials.FirstOrDefault(s => s.id == serial.id);
                    if (serialInTotalList != null)
                    {
                        serialInList.title = serial.title;
                        serialInList.image_url = serial.image_url;
                        serialInList.rating = serial.rating;
                        serialInList.year = serial.year;
                        serialInList.description = serial.description;
                        serialInList.short_description = serial.short_description;
                        serialInList.age_limit = serial.age_limit;
                        serialInList.countries = serial.countries;
                        serialInList.genres = serial.genres;
                        serialInList.director = serial.director;
                    }

                    var serialInMoviesFromDb = mainWindowDataContext.SerialsViewModel.serialsFromDb.FirstOrDefault(s => s.id == serial.id);
                    if (serialInMoviesFromDb != null)
                    {
                        serialInMoviesFromDb.title = serial.title;
                        serialInMoviesFromDb.image_url = serial.image_url;
                        serialInMoviesFromDb.rating = serial.rating;
                        serialInMoviesFromDb.year = serial.year;
                        serialInMoviesFromDb.description = serial.description;
                        serialInMoviesFromDb.short_description = serial.short_description;
                        serialInMoviesFromDb.age_limit = serial.age_limit;
                        serialInMoviesFromDb.countries = serial.countries;
                        serialInMoviesFromDb.genres = serial.genres;
                        serialInMoviesFromDb.director = serial.director;
                    }

                    App.UpdateView(mainWindow);
                    mainWindowDataContext.MovieViewModel.UpdateData();


                    var serialFromDb = unitOfWorkContent.Serials.Get(serial.id ?? 0);

                    if (serialFromDb != null)
                    {
                        serialFromDb.title = serial.title;
                        serialFromDb.image_url = serial.image_url;
                        serialFromDb.rating = serial.rating;
                        serialFromDb.year = serial.year;
                        serialFromDb.description = serial.description;
                        serialFromDb.short_description = serial.short_description;
                        serialFromDb.age_limit = serial.age_limit;
                        serialFromDb.countries = serial.countries;
                        serialFromDb.genres = serial.genres;
                        serialFromDb.director = serial.director;

                        unitOfWorkContent.Serials.Update(serialFromDb);
                    }

                    mainWindowDataContext.SerialsViewModel.UpdateData();

                    Close();
                }
            }


        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
