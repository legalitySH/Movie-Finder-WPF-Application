using DevExpress.Mvvm.Native;
using MovieFinder.Database;
using MovieFinder.Database.Repositories;
using MovieFinder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MovieFinder.View.Admin.UserManagment
{
    /// <summary>
    /// Логика взаимодействия для UserManagment.xaml
    /// </summary>
    public partial class UserManagment : Window, INotifyPropertyChanged
    {
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private MainWindow mainWindow;

        private ObservableCollection<BannedUser> _bannedUsers;
        public ObservableCollection<BannedUser> BannedUsers
        {
            get
            {
                return _bannedUsers;
            }
            set
            {
                _bannedUsers = value;
                OnPropertyChanged(nameof(BannedUsers));   
            }
        }


        public User SelectedUser {  get; set; }
        public BannedUser SelectedBannedUser { get; set; }

        private UnitOfWorkContent unitOfWorkContent;

        public UserManagment()
        {
            InitializeComponent();
            DataContext = this;
            unitOfWorkContent = new UnitOfWorkContent();
            Users = unitOfWorkContent.Users.GetAll().ToObservableCollection();
            BannedUsers = unitOfWorkContent.Banned.GetAll().ToObservableCollection();
            Users.CollectionChanged += Users_CollectionChanged;
            mainWindow = App.Current.MainWindow as MainWindow;
            App.Current.MainWindow = this;
            Closed += UserManagment_Closed;
        }

        private void UserManagment_Closed(object? sender, EventArgs e)
        {
            App.Current.MainWindow = mainWindow;
            Close();
        }

        private void Users_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            Users = unitOfWorkContent.Users.GetAll().ToObservableCollection();
            BannedUsers = unitOfWorkContent.Banned.GetAll().ToObservableCollection();
        }

        private void BlockUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedUser == null)
            {
                App.Notifier.ShowInformation("Выберите пользователя,которого хотите заблокировать!");
                return;
            }
            if(unitOfWorkContent.Banned.GetByUserId(SelectedUser.id) != null)
            {
                App.Notifier.ShowError("Данный пользователь уже заблокирован!");
                return;
            }
            var banned = new BannedUser() { user_id = SelectedUser.id };
            unitOfWorkContent.Banned.Add(banned);
            UpdateData();
        }

        private void UnBlockUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedBannedUser == null)
            {
                App.Notifier.ShowInformation("Выберите пользователя, которого вы хотите разблокировать из списка заблокированных");
                return;
            }
            else
            {
                var bannedUser = unitOfWorkContent.Banned.Get(SelectedBannedUser.id);
                if(bannedUser != null)
                {
                    unitOfWorkContent.Banned.Delete(bannedUser.id);
                    UpdateData();
                }
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
