using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage(UsersViewModel usersViewModel)
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

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            User newUser = new User()
            {
                login = LoginTextBox.Text,
                password = PasswordTextBox.Password,
                email = MailTextBox.Text,
                role = "admin"
            };

            var dataContext = DataContext as UsersViewModel;

            dataContext.AddUserCommand.Execute(newUser);
        }
    }
}
