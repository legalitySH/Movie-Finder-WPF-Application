using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace MovieFinder.Attributes
{
    public class MailExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string))
                return false;

            try
            {
                MailAddress mail = new MailAddress((string)value);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
