using MovieFinder.Attributes;
using MovieFinder.Classes;
using System.ComponentModel.DataAnnotations;

namespace MovieFinder.Models
{
    public class User
    {
        public int id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Пожалуйста, заполните поля с логином")]
        [LatinOnly(ErrorMessage = "Логин должен содержать только латинские символы или цифры и иметь длину не более 20 символов")]
        public string login { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Пожалуйста, введите пароль")]
        [PasswordValidation(ErrorMessage = "Пароль должен содержать не менее 4 символов и хотя бы один специальный знак")]
        public string password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Пожалуйста, укажите почту для регистрации")]
        [MailExists(ErrorMessage = "Mail адрес не существует, повторите попытку с существующей почтой")]
        public string email { get; set; }

        public string role { get; set; }
        public DateTime registration_date { get; set; }


        public User()
        {
            registration_date = DateTime.Now;
            role = "user";
        }
    }
}
