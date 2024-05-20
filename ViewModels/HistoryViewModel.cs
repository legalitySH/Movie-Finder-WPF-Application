using MovieFinder.Database;
using MovieFinder.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace MovieFinder.ViewModels
{
    public class HistoryViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ObservableCollection<HistoryModel> HistoryList { get; set; }
        public ObservableCollection<HistoryModel> TotalHistories { get; set; }

        public bool IsPaginationVisible { get; set; }


        private UnitOfWorkContent UnitOfWorkContent;

        public ICommand? AddHistoryCommand { get; set; }

        public HistoryModel SelectedHistory { get; set; }


        public HistoryViewModel(UnitOfWorkContent UnitOfWorkContent)
        {

            this.UnitOfWorkContent = UnitOfWorkContent;
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                HistoryList = new ObservableCollection<HistoryModel>(UnitOfWorkContent.History.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
                TotalHistories = new ObservableCollection<HistoryModel>(UnitOfWorkContent.History.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
            }
            else
            {
                HistoryList = new ObservableCollection<HistoryModel>();
                TotalHistories = new ObservableCollection<HistoryModel>();
            }

        }

        public void UpdateData()
        {
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                HistoryList = new ObservableCollection<HistoryModel>(UnitOfWorkContent.History.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
                TotalHistories = new ObservableCollection<HistoryModel>(UnitOfWorkContent.History.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
            }
            else
            {
                HistoryList = new ObservableCollection<HistoryModel>();
                TotalHistories = new ObservableCollection<HistoryModel>();
            }
        }

    }
}