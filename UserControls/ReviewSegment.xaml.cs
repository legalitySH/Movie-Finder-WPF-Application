using MovieFinder.Database;
using MovieFinder.Models;
using MovieFinder.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieFinder.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ReviewSegment.xaml
    /// </summary>
    public partial class ReviewSegment : UserControl
    {
        private UnitOfWorkContent unit;


        public ReviewSegment()
        {
            InitializeComponent();
            DataContextChanged += ReviewSegment_DataContextChanged;
            unit = new UnitOfWorkContent();
        }

        private void ReviewSegment_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = DataContext as Review;

            string login = unit.Users.Get(dataContext.user_id).login;

            if (login == null)
            {
                ReviewViewModel reviewViewModel = new ReviewViewModel();
                reviewViewModel.RemoveReview(dataContext);
                this.Visibility = Visibility.Hidden;

            }

            LoginLetter.Text = login.Substring(0, 1).ToUpper();

            ReviewText.Text = dataContext.review_text;

            var colorDict = new Dictionary<char, Color>()
                {
                    {'A', Color.FromRgb(0, 96, 0)},
                    {'B', Color.FromRgb(255, 0, 0)},
                    {'C', Color.FromRgb(0, 255, 0)},
                    {'D', Color.FromRgb(0, 0, 255)},
                    {'E', Color.FromRgb(255, 255, 0)},
                    {'F', Color.FromRgb(255, 0, 255)},
                    {'G', Color.FromRgb(0, 255, 255)},
                    {'H', Color.FromRgb(128, 0, 0)},
                    {'I', Color.FromRgb(0, 128, 0)},
                    {'J', Color.FromRgb(0, 0, 128)},
                    {'K', Color.FromRgb(128, 128, 0)},
                    {'L', Color.FromRgb(128, 0, 128)},
                    {'M', Color.FromRgb(0, 128, 128)},
                    {'N', Color.FromRgb(192, 192, 192)},
                    {'O', Color.FromRgb(128, 128, 128)},
                    {'P', Color.FromRgb(255, 0, 0)},
                    {'Q', Color.FromRgb(0, 255, 0)},
                    {'R', Color.FromRgb(0, 0, 255)},
                    {'S', Color.FromRgb(255, 255, 0)},
                    {'T', Color.FromRgb(255, 0, 255)},
                    {'U', Color.FromRgb(0, 255, 255)},
                    {'V', Color.FromRgb(128, 0, 0)},
                    {'W', Color.FromRgb(0, 128, 0)},
                    {'X', Color.FromRgb(0, 0, 128)},
                    {'Y', Color.FromRgb(128, 128, 0)},
                    {'Z', Color.FromRgb(128, 0, 128)}
                };
            if (colorDict.ContainsKey(LoginLetter.Text[0]))
            {
                Color color = colorDict[LoginLetter.Text[0]];
                LogoColor.Background = new SolidColorBrush(color);
            }
            else
            {
                LogoColor.Background = new SolidColorBrush(Colors.Gray);
            }

            var user = unit.Users.Get(dataContext.user_id);
            if(user != null)
            {
                Login.Text = user.login;
            }


        }
    }
}
