using MovieFinder.Attributes;
using MovieFinder.View;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications;


namespace MovieFinder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    ///                   

    public partial class MainWindow : Window
    {
        private Notifier notifier;


        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(CustomCommands.RegistrationCommand, RegistrationCommand_Executed, RegistrationCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(CustomCommands.AuthorizationCommand, AuthorizationCommand_Executed, AuthorizationCommand_CanExecute));



            Uri darkThemeUri = new Uri("/MovieFinder;component/Resourses/Themes/DarkPurpleTheme.xaml", UriKind.Relative);
            App.Theme = darkThemeUri;




        }


        private void RegistrationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dataContext = DataContext as MainWindowViewModel;
            RegisterPage registerPage = new RegisterPage(dataContext.UsersViewModel);
            registerPage.RegisterGrid.MouseDown += (s, e) => navigationFrame.Content = null;
            registerPage.ExitBtn.Click += (s, e) => navigationFrame.Content = null;
            registerPage.authLink.Click += (s, e) => AuthorizationCommand_Executed(s, null);
            navigationFrame.Content = registerPage;
        }


        private void RegistrationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            return;
        }


        private void AuthorizationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dataContext = DataContext as MainWindowViewModel;
            AuthorizationPage authorizationPage = new AuthorizationPage(dataContext.UsersViewModel);
            authorizationPage.RegisterGrid.MouseDown += (s, e) => navigationFrame.Content = null;
            authorizationPage.ExitBtn.Click += (s, e) => navigationFrame.Content = null;
            authorizationPage.registerLink.Click += (s, e) => RegistrationCommand_Executed(s, null);
            navigationFrame.Content = authorizationPage;
        }


        private void AuthorizationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            return;
        }


        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dataContext = DataContext as MainWindowViewModel;
            var textbox = sender as TextBox;
            if (searchTextBox.Text == string.Empty && dataContext != null)
            {
                dataContext.MovieViewModel.UpdatePaginationControl();
                dataContext.SerialsViewModel.UpdatePaginationControl();

                dataContext.MovieViewModel.IsPaginationVisible = true;
                dataContext.SerialsViewModel.IsPaginationVisible = true;

                dataContext.MovieViewModel.isSearching = false;
                dataContext.SerialsViewModel.isSearching = false;
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs? e)
        {
            var dataContext = DataContext as MainWindowViewModel;
            dataContext.MovieViewModel.IsPaginationVisible = false;
            dataContext.SerialsViewModel.IsPaginationVisible = false;

            dataContext.movieCardsPage.paginationControl.Visibility = Visibility.Hidden;
            dataContext.serialsCardPage.paginationControl.Visibility = Visibility.Hidden;

            if (dataContext != null)
            {
                switch (dataContext.CurrentPage.GetType().Name)
                {
                    case "MovieCardsPage":
                        {
                            dataContext.SerialsViewModel.SerialList = dataContext.SerialsViewModel.serialsFromDb;
                            dataContext.MovieViewModel.SearchMovieCommand.Execute(searchTextBox.Text);

                            break;
                        }
                    case "SerialsCardPage":
                        {
                            dataContext.SerialsViewModel.SerialList = dataContext.SerialsViewModel.serialsFromDb;
                            dataContext.MovieViewModel.SearchMovieCommand.Execute(String.Empty);
                            break;
                        }
                }
            }
        }

        private void Auth_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = DataContext as MainWindowViewModel;

            bool isAuthorized = dataContext.UsersViewModel.IsAuthorizedNonStatic;

            if (isAuthorized)
            {
                MainWindowViewModel newViewModel = new MainWindowViewModel();
                newViewModel.UsersViewModel.IsAuthorizedNonStatic = true;
                newViewModel.UsersViewModel.IsGuestModeNonStatic = false;
                newViewModel.AuthorizedUserNonStatic = MainWindowViewModel.AuthorizedUser;
                DataContext = newViewModel;
                
                if(MainWindowViewModel.AuthorizedUser.role == "admin")
                {
                    AdminPanel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MainWindowViewModel newViewModel = new MainWindowViewModel();
                newViewModel.UsersViewModel.IsAuthorizedNonStatic = false;
                newViewModel.UsersViewModel.IsGuestModeNonStatic = true;
                newViewModel.AuthorizedUserNonStatic = null;
                DataContext = newViewModel;
                AdminPanel.Visibility = Visibility.Hidden;
            }



        }

        private void navigationFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                searchBtn_Click(sender, e);
            }
        }
    }
}
