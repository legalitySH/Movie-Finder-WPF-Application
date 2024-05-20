using MovieFinder.ViewModels;
using System.Data;
using System.Globalization;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
namespace MovieFinder
{

    public partial class App : Application
    {
        private static List<CultureInfo> _languages = new List<CultureInfo>();

        private static Uri _theme;

        public static Notifier Notifier
        {
            get
            {

                return new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: App.Current.MainWindow,
                        corner: Corner.TopRight,
                        offsetX: 10,
                        offsetY: 10);

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                        notificationLifetime: TimeSpan.FromSeconds(3),
                        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                    cfg.Dispatcher = Application.Current.Dispatcher;
                });
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _languages.Clear();
            _languages.Add(new CultureInfo("en-US")); // Культура по умолчанию
            _languages.Add(new CultureInfo("ru-RU"));

        }

        public static Uri Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                if (value == null) { return; }
                if (value == _theme) { return; }

                _theme = value;

                var dict = new ResourceDictionary() { Source = value };

                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.Contains("PurpleTheme")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

            }
        }

        public static CultureInfo Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentException("Language Argument Exception");
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                // Меняем язык приложения

                Thread.CurrentThread.CurrentUICulture = value;

                // Создаем ResourseDictionary для новой культуры

                var dict = new ResourceDictionary();

                switch (value.Name)
                {
                    case "en-US":
                        {
                            dict.Source = new Uri(string.Format("pack://application:,,,/Resourses/Languages/lang.{0}.xaml", value.Name));
                            break;
                        }
                    default:
                        {
                            dict.Source = new Uri(string.Format("pack://application:,,,/Resourses/Languages/lang.xaml", value.Name));
                            break;
                        }
                }

                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("pack://application:,,,/Resourses/Languages/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
            }
        }



        public static List<CultureInfo> Languages
        {
            get
            {
                return _languages;
            }

        }

        public App()
        {

        }

        public static void UpdateView(MainWindow window)
        {
            MainWindowViewModel? viewModel = window.DataContext as MainWindowViewModel;

            if(viewModel != null) {
                viewModel.movieCardsPage = new View.MovieCardsPage(viewModel.MovieViewModel);
                viewModel.serialsCardPage = new View.SerialsCardPage(viewModel.SerialsViewModel);
                viewModel.favouritesPage = new View.FavouritesPage(viewModel.FavouriteViewModel);
                viewModel.historyPage = new View.HistoryPage(viewModel.HistoryViewModel);

            }
        }
    }

}