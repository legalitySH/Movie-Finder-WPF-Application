using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MovieFinder.Attributes
{
    public class LatinOnlyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string))
                return false;

            string login = (string)value;

            if (!Regex.IsMatch(login, "^[a-zA-Z0-9]+$"))
                return false;

            if (login.Length > 20)
                return false;

            return true;
        }
    }
}
