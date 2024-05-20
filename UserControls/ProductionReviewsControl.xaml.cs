using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;
using ToastNotifications.Messages;

namespace MovieFinder.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ProductionReviewsControl.xaml
    /// </summary>
    public partial class ProductionReviewsControl : UserControl
    {
        private ReviewViewModel reviewViewModel;
        public ProductionReviewsControl()
        {
            InitializeComponent();
            reviewViewModel = new ReviewViewModel();
            DataContextChanged += ProductionReviewsControl_DataContextChanged;
        }


        private void ReviewsListLoad()
        {
            if (DataContext == null) return;
            UnitOfWorkContent unitOfWorkContent = new UnitOfWorkContent();


            if (DataContext is Movie)
            {
                var dataContext = DataContext as Movie;
                List<Review> reviews = unitOfWorkContent.Reviews.GetAllByProduction("movie", dataContext.id);
                ReviewsListBox.ItemsSource = reviews;
            }
            else if (DataContext is Serial)
            {
                var dataContext = DataContext as Serial;
                List<Review> reviews = unitOfWorkContent.Reviews.GetAllByProduction("serial", dataContext.id ?? 0);
                ReviewsListBox.ItemsSource = reviews;
            }

        }

        private void ProductionReviewsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReviewsListLoad();
        }

        private void AddReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.AuthorizedUser != null)
            {
                if (DataContext is Movie)
                {
                    var dataContext = DataContext as Movie;
                    var review = new Review()
                    {
                        type = "movie",
                        production_id = dataContext.id,
                        review_text = ReviewTextBox.Text,
                        user_id = MainWindowViewModel.AuthorizedUser.id
                    };

                    reviewViewModel.AddReviewCommand.Execute(review);
                    ReviewsListLoad();

                }
                else if (DataContext is Serial)
                {
                    var dataContext = DataContext as Serial;
                    var review = new Review()
                    {
                        type = "serial",
                        production_id = dataContext.id ?? 0,
                        review_text = ReviewTextBox.Text,
                        user_id = MainWindowViewModel.AuthorizedUser.id
                    };

                    reviewViewModel.AddReviewCommand.Execute(review);
                    ReviewsListLoad();

                }
            }
            else
            {
                App.Notifier.ShowError("Оставлять отзывы могут только авторизованные пользователи");
            }
        }
    }
}
