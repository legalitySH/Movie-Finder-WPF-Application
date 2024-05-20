using GalaSoft.MvvmLight.Command;
using MovieFinder.Classes;
using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Messages;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;


namespace MovieFinder.ViewModels
{
    public class UsersViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ObservableCollection<User> UserList { get; set; }
        public ObservableCollection<User> TotalUsers { get; set; }

        private static bool _isGuestMode;
        private static bool _isAuthorized;
        public static bool _isAdminMode;

        public static bool IsGuestMode
        {
            get
            {
                return _isGuestMode;
            }
            set
            {
                _isGuestMode = value;
            }
        }

        public static bool IsAuthorized
        {
            get
            {
                return _isAuthorized;
            }
            set
            {
                _isAuthorized = value;
            }
        }


        public static bool IsAdminMode
        {
            get
            {
                return _isAdminMode;
            }
            set
            {
                _isAdminMode = value;
            }
        }


        public bool IsGuestModeNonStatic { get; set; }

        public bool IsAuthorizedNonStatic { get; set; }

        public bool IsAdminNonStatic { get; set; }



        private UnitOfWorkContent UnitOfWorkContent;

        public ICommand? AddUserCommand { get; set; }
        public ICommand? AuthorizeUserCommand { get; set; }
        public ICommand? LogoutUserCommand { get; set; }
        public ICommand? RemoveUserCommand { get; set; }
        public ICommand? UpdateUserCommand { get; set; }

        public User SelectedUserModel { get; set; }


        public UsersViewModel(UnitOfWorkContent UnitOfWorkContent)
        {
            this.UnitOfWorkContent = UnitOfWorkContent;
            UserList = new ObservableCollection<User>(UnitOfWorkContent.Users.GetAll());
            TotalUsers = new ObservableCollection<User>(UnitOfWorkContent.Users.GetAll());

            AddUserCommand = new RelayCommand<User>(AddUser);
            AuthorizeUserCommand = new RelayCommand<User>(AuthorizateUser);
            LogoutUserCommand = new RelayCommand(LogoutUser);

            IsGuestMode = true;
            IsGuestModeNonStatic = true;
            IsAuthorized = false;
            IsAuthorizedNonStatic = false;
            IsAdminMode = false;
            IsAdminNonStatic = false;
        }

        public async void AddUser(User user)
        {

            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(user, new ValidationContext(user), results, true);

            if (!isValid)
            {
                string errorMessage = results[0].ErrorMessage;
                App.Notifier.ShowError(errorMessage);
                return;
            }
            else
            {
                if (UnitOfWorkContent.Users.isExistsByMail(user.email))
                {
                    App.Notifier.ShowError("Пользователь с таким email уже существует!");
                    return;
                }
                if (UnitOfWorkContent.Users.isExistsByLogin(user.login))
                {
                    App.Notifier.ShowError("Пользователь с таким логином уже существует!");
                    return;
                }

                var newUser = new User()
                {
                    login = user.login,
                    password = PasswordHasher.HashPassword(user.password),
                    email = user.email
                };

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("moviefinder@mail.ru", "MovieFinder Application");
                mail.To.Add(new MailAddress(newUser.email, newUser.login));
                string authCode = GenerateLoginCode();
                mail.Subject = "Код для авторизации в приложение MovieFinder";
                mail.Body = $"Код для регистрации: {authCode}";
                try
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Host = "smtp.mail.ru";
                        client.Port = 25;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential("moviefinder@mail.ru", "x0C042ctjJ8Gaj2rnp3V");

                        await client.SendMailAsync(mail);
                    }
                }
                catch (Exception ex)
                {
                    App.Notifier.ShowError("Ошибка при отправке письма!");
                    return;
                }

                MailConfirmWindow mailConfirmWindow = new MailConfirmWindow(authCode, newUser.email);

                if (mailConfirmWindow.ShowDialog() == true)
                {
                    UnitOfWorkContent.Users.Add(newUser);
                    App.Notifier.ShowSuccess("Поздравляем с успешной регистрацией!");
                    if (UnitOfWorkContent.Users.GetByLogin(newUser.login) != null)
                    {
                        var authUser = UnitOfWorkContent.Users.GetByLogin(newUser.login);
                        MainWindowViewModel.AuthorizedUser = authUser;
                        IsGuestMode = false;
                        IsGuestModeNonStatic = false;
                        IsAuthorized = true;
                        IsAuthorizedNonStatic = true;

                        if(authUser.role == "admin")
                        {
                            IsAdminMode = true;
                            IsAdminNonStatic = true;
                        }

                    }
                    if (App.Current.MainWindow as MainWindow != null)
                    {
                        MainWindow mainWindow = App.Current.MainWindow as MainWindow;
                        mainWindow.navigationFrame.Content = null;
                    }
                }
                else
                {

                    App.Notifier.ShowError("Не удалось подтвердить почту, попробуйте снова");
                    return;
                }
            }

            return;
        }

        public void AuthorizateUser(User user)
        {
            var foundedUser = UnitOfWorkContent.Users.GetByLogin(user.login);

            if (foundedUser != null && PasswordHasher.VerifyPassword(user.password, foundedUser.password))
            {
                if(UnitOfWorkContent.Banned.GetByUserId(foundedUser.id) != null)
                {
                    App.Notifier.ShowError("Вы были заблокированы администратором!");
                    return;
                }
                App.Notifier.ShowSuccess("Успешная авторизация!");
                MainWindowViewModel.AuthorizedUser = foundedUser;

                IsGuestMode = false;
                IsGuestModeNonStatic = false;
                IsAuthorized = true;
                IsAuthorizedNonStatic = true;
                if (foundedUser.role == "admin")
                {
                    IsAdminMode = true;
                }


                

                MainWindow mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.navigationFrame.Content = null;

                return;
            }
            else
            {
                App.Notifier.ShowError("Не удалось найти пользователя с таким логином или паролем!");
            }
        }

        public void LogoutUser()
        {
            MainWindowViewModel.AuthorizedUser = null;
            IsGuestMode = true;
            IsGuestModeNonStatic = true;
            IsAuthorized = false;
            IsAuthorizedNonStatic = false;
        }


        public string GenerateLoginCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 9999);

            string loginCode = code.ToString("D4");

            return loginCode;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}