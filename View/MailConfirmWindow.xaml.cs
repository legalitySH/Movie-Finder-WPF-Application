using System.Windows;
using System.Windows.Controls;

namespace MovieFinder.View
{
    /// <summary>
    /// Логика взаимодействия для MailConfirmWindow.xaml
    /// </summary>
    public partial class MailConfirmWindow : Window
    {
        private string confirmCode;
        public MailConfirmWindow(string code, string mail)
        {
            InitializeComponent();
            this.confirmCode = code;
            SendingMessage.Text = $"На почтовый ящик {mail} было отправлено письмо с кодом для подтверждения регистрации, пожалуйста введите его в поле ниже:";
        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CodeTextBox.Text == confirmCode)
            {
                DialogResult = true;
                Close();

            }
        }
    }
}
