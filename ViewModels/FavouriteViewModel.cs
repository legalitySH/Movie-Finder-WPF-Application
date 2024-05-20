using GalaSoft.MvvmLight.Command;
using MovieFinder.Database;
using MovieFinder.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace MovieFinder.ViewModels
{
    public class FavouriteViewModel : ViewModelBase
    {
        public ObservableCollection<FavouriteModel> FavouriteList { get; set; }
        public ObservableCollection<FavouriteModel> TotalFavourites { get; set; }

        public ICommand? AddToFavourite { get; set; }

        public bool IsPaginationVisible { get; set; }


        private UnitOfWorkContent UnitOfWorkContent;


        public FavouriteModel SelectedFavourite { get; set; }


        public FavouriteViewModel(UnitOfWorkContent UnitOfWorkContent)
        {

            this.UnitOfWorkContent = UnitOfWorkContent;
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                FavouriteList = new ObservableCollection<FavouriteModel>(UnitOfWorkContent.Favourites.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
                TotalFavourites = new ObservableCollection<FavouriteModel>(UnitOfWorkContent.Favourites.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
            }
            else
            {
                FavouriteList = new ObservableCollection<FavouriteModel>();
                TotalFavourites = new ObservableCollection<FavouriteModel>();
            }

            AddToFavourite = new RelayCommand<FavouriteModel>(FavouriteInserting);
        }

        public void FavouriteInserting(FavouriteModel favourite)
        {
            if (!UnitOfWorkContent.Favourites.isExistsTotal(favourite.production_id, favourite.type, MainWindowViewModel.AuthorizedUser.id))
            {
                UnitOfWorkContent.Favourites.Add(favourite);
            }
        }

        public void UpdateData()
        {
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                FavouriteList = new ObservableCollection<FavouriteModel>(UnitOfWorkContent.Favourites.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
                TotalFavourites = new ObservableCollection<FavouriteModel>(UnitOfWorkContent.Favourites.GetAllByUserId(MainWindowViewModel.AuthorizedUser.id));
            }
            else
            {
                FavouriteList = new ObservableCollection<FavouriteModel>();
                TotalFavourites = new ObservableCollection<FavouriteModel>();
            }

        }

    }
}