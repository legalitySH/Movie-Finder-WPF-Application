using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage(UsersViewModel usersViewModel)
        {
            InitializeComponent();
            DataContext = usersViewModel;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var dataContextOfMain = mainWindow.DataContext as MainWindowViewModel;

                mainWindow.navigationFrame.Visibility = Visibility.Visible;
            }
        }

        private void AuthorizationBtn_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as UsersViewModel;

            var user = new User()
            {
                login = LoginTextBox.Text,
                password = PasswordTextBox.Password,
            };

            dataContext.AuthorizateUser(user);
        }
    }
}
