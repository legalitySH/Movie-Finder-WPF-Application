using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieFinder.UserControls
{
    public partial class FilmCardControl : UserControl
    {
        private Brush prevColor;

        public static readonly RoutedEvent DirectEvent =
            EventManager.RegisterRoutedEvent("DirectEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FilmCardControl));

        public event RoutedEventHandler Direct
        {
            add { AddHandler(DirectEvent, value); }
            remove { RemoveHandler(DirectEvent, value); }
        }

        public FilmCardControl()
        {
            InitializeComponent();
            MouseEnter += FilmCardControl_MouseEnter;
            MouseLeave += FilmCardControl_MouseLeave;
        }

        private void FilmCardControl_MouseEnter(object sender, MouseEventArgs e)
        {
            MovieTitle.FontSize += 2;
            prevColor = MovieTitle.Foreground;
            MovieTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f5a913"));


            RoutedEventArgs eventArgs = new RoutedEventArgs(DirectEvent, this);
            RaiseEvent(eventArgs);
        }

        private void FilmCardControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MovieTitle.FontSize = MovieTitle.FontSize - 2;
            if (App.Theme.OriginalString.Contains("LightPurple"))
            {
                MovieTitle.Foreground = Brushes.Black;
            }
            else
            {
                MovieTitle.Foreground = Brushes.White;
            }

        }
    }
}