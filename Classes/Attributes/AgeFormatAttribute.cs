using System.ComponentModel.DataAnnotations;

public class AgeFormatAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || !(value is string))
            return false;

        string strValue = (string)value;

        return System.Text.RegularExpressions.Regex.IsMatch(strValue, @"^age\d{1,2}$");
    }
}