using System.Windows;
using System.Windows.Controls;


namespace MovieFinder.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PaginationControl.xaml
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        public int pageCount;
        public PaginationControl()
        {
            InitializeComponent();
            leftButton.IsEnabled = false;
        }

        public int getCurrentPage()
        {
            return Convert.ToInt32(currentPage.Text);
        }

        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            int currentPageInt = Convert.ToInt32(currentPage.Text);

            if (currentPageInt - 1 == 1)
            {
                leftButton.IsEnabled = false;
            }

            if (currentPageInt - 1 >= 1)
            {
                currentPageInt--;
                currentPage.Text = currentPageInt.ToString();
                rightButton.IsEnabled = true;
            }
            else
            {
                leftButton.IsEnabled = false;
            }

        }

        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            int currentPageInt = Convert.ToInt32(currentPage.Text);
            if (currentPageInt + 1 == pageCount)
            {
                rightButton.IsEnabled = false;
            }
            if (currentPageInt + 1 <= pageCount)
            {
                currentPageInt++;
                currentPage.Text = currentPageInt.ToString();
                leftButton.IsEnabled = true;
            }
            else
            {
                rightButton.IsEnabled = false;
            }

        }
    }
}
