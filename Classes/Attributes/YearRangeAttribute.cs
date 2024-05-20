using System.ComponentModel.DataAnnotations;

namespace MovieFinder.Attributes
{
    public class YearRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is int))
                return false;

            int intValue = (int)value;
            int currentYear = DateTime.Now.Year;


            return intValue >= 1900 && intValue <= currentYear;
        }
    }
}
