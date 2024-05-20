using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static System.Net.WebRequestMethods;

namespace MovieFinder.View.Admin.Edit
{
    /// <summary>
    /// Логика взаимодействия для EditMovie.xaml
    /// </summary>
    public partial class EditMovie : Window
    {
        private MainWindow mainWindow;

        bool isEdited = false;

        public EditMovie(Movie movie)
        {
            InitializeComponent();
            DataContext = movie;
            RussianName.Text = movie.russian_name;
            OriginalName.Text = movie.original_name;
            Url.TextChanged += Url_TextChanged;
            Url.Text = movie.image_url;
            Rating.Text = movie.rating.ToString();
            Year.Text = movie.year.ToString();
            Duration.Text = movie.duration.ToString();
            Description.Text = movie.description;
            ShortDescription.Text = movie.short_description;
            AgeLimit.Text = movie.age_limit.ToString();
            Countries.Text = movie.countries;
            Genres.Text = movie.genres;
            Director.Text = movie.director;


            mainWindow = App.Current.MainWindow as MainWindow;
            App.Current.MainWindow = this;

            Closed += EditMovie_Closed;
            


        }

        private void EditMovie_Closed(object? sender, EventArgs e)
        {
            App.Current.MainWindow = mainWindow;
            if(isEdited)
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
            var dataContext = DataContext as Movie;
            float rating = 0;
            string uri;
            int year = 0;
            int age = 0;
            int duration = 0;
            try
            {
                rating = Convert.ToSingle(Rating.Text);
            }
            catch(Exception ex)
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
            catch(Exception ex)
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
                id = dataContext.id,
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

            bool isValid = Validator.TryValidateObject(movie, new ValidationContext(movie), results,true);

            if(!isValid)
            {
                App.Notifier.ShowError(results[0].ErrorMessage);
            }
            else
            {
                isEdited = true;

                var unitOfWorkContent = new UnitOfWorkContent();

                var movieToEdit = unitOfWorkContent.Movies.Get(dataContext.id);

                if(movieToEdit != null)
                {
                    var mainWindowDataContext = mainWindow.DataContext as MainWindowViewModel;

                    // Обновление значения Movie в MoviesList
                    var movieInList = mainWindowDataContext.MovieViewModel.MoviesList.FirstOrDefault(m => m.id == movie.id);
                    if (movieInList != null)
                    {
                        movieInList.russian_name = movie.russian_name;
                        movieInList.original_name = movie.original_name;
                        movieInList.image_url = movie.image_url;
                        movieInList.rating = movie.rating;
                        movieInList.year = movie.year;
                        movieInList.duration = movie.duration;
                        movieInList.description = movie.description;
                        movieInList.short_description = movie.short_description;
                        movieInList.age_limit = movie.age_limit;
                        movieInList.countries = movie.countries;
                        movieInList.genres = movie.genres;
                        movieInList.director = movie.director;
                    }

                    // Обновление значения Movie в totalList
                    var movieInTotalList = mainWindowDataContext.MovieViewModel.totalMovies.FirstOrDefault(m => m.id == movie.id);
                    if (movieInTotalList != null)
                    {
                        movieInTotalList.russian_name = movie.russian_name;
                        movieInTotalList.original_name = movie.original_name;
                        movieInTotalList.image_url = movie.image_url;
                        movieInTotalList.rating = movie.rating;
                        movieInTotalList.year = movie.year;
                        movieInTotalList.duration = movie.duration;
                        movieInTotalList.description = movie.description;
                        movieInTotalList.short_description = movie.short_description;
                        movieInTotalList.age_limit = movie.age_limit;
                        movieInTotalList.countries = movie.countries;
                        movieInTotalList.genres = movie.genres;
                        movieInTotalList.director = movie.director;
                    }

                    // Обновление значения Movie в MoviesFromDb
                    var movieInMoviesFromDb = mainWindowDataContext.MovieViewModel.moviesFromDb.FirstOrDefault(m => m.id == movie.id);
                    if (movieInMoviesFromDb != null)
                    {
                        movieInMoviesFromDb.russian_name = movie.russian_name;
                        movieInMoviesFromDb.original_name = movie.original_name;
                        movieInMoviesFromDb.image_url = movie.image_url;
                        movieInMoviesFromDb.rating = movie.rating;
                        movieInMoviesFromDb.year = movie.year;
                        movieInMoviesFromDb.duration = movie.duration;
                        movieInMoviesFromDb.description = movie.description;
                        movieInMoviesFromDb.short_description = movie.short_description;
                        movieInMoviesFromDb.age_limit = movie.age_limit;
                        movieInMoviesFromDb.countries = movie.countries;
                        movieInMoviesFromDb.genres = movie.genres;
                        movieInMoviesFromDb.director = movie.director;
                    }


                    var movieFromDb = unitOfWorkContent.Movies.Get(movie.id);

                    if(movieFromDb != null)
                    {
                        movieFromDb.russian_name = movie.russian_name;
                        movieFromDb.original_name = movie.original_name;
                        movieFromDb.image_url = movie.image_url;
                        movieFromDb.rating = movie.rating;
                        movieFromDb.year = movie.year;
                        movieFromDb.duration = movie.duration;
                        movieFromDb.description = movie.description;
                        movieFromDb.short_description = movie.short_description;
                        movieFromDb.age_limit = movie.age_limit;
                        movieFromDb.countries = movie.countries;
                        movieFromDb.genres = movie.genres;
                        movieFromDb.director = movie.director;

                        unitOfWorkContent.Movies.Update(movieFromDb);
                    }

                    mainWindowDataContext.MovieViewModel.UpdateData();


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
