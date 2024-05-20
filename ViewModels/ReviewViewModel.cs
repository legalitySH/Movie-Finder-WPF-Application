using GalaSoft.MvvmLight.Command;
using MovieFinder.Database;
using MovieFinder.Models;
using System.Windows.Input;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace MovieFinder.ViewModels
{
    public class ReviewViewModel : ViewModelBase
    {

        public ICommand? AddReviewCommand { get; set; }
        public ICommand? RemoveReviewCommand { get; set; }

        private UnitOfWorkContent UnitOfWorkContent;



        public ReviewViewModel(UnitOfWorkContent UnitOfWorkContent)
        {

            this.UnitOfWorkContent = UnitOfWorkContent;
            AddReviewCommand = new RelayCommand<Review>(AddReview);
            RemoveReviewCommand = new RelayCommand<Review>(RemoveReview);
        }

        public ReviewViewModel()
        {

            this.UnitOfWorkContent = new UnitOfWorkContent();
            AddReviewCommand = new RelayCommand<Review>(AddReview);
            RemoveReviewCommand = new RelayCommand<Review>(RemoveReview);
        }


        public void AddReview(Review review)
        {
            UnitOfWorkContent.Reviews.Add(review);
        }

        public void RemoveReview(Review review)
        {
            UnitOfWorkContent.Reviews.DeleteByReview(review);
        }


    }
}