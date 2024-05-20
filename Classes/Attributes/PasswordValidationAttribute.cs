using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MovieFinder.Classes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string))
                return false;

            string password = (string)value;

            if (password.Length < 4)
                return false;

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return false;

            return true;
        }
    }
}