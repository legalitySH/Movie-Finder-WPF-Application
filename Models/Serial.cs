using MovieFinder.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MovieFinder.Models
{
    public class Serial
    {
        public int? id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Название не задано")]
        public string? title { get; set; }

        public string? image_url { get; set; }

        [Range(0, 10, ErrorMessage = "Рейтинг должен быть в диапазоне от 0 до 5")]
        public double rating { get; set; }

        [YearRange(ErrorMessage = "Некорректный год выпуска")]
        public int year { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Описание не задано!")]
        public string? description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Краткое описание не задано!")]
        public string? short_description { get; set; }

        [Range(0, 18, ErrorMessage = "Некорректный формат возрастного ограничения(0-18)")]
        public int age_limit { get; set; }


        public string? countries { get; set; }
        public string? genres { get; set; }

        [Required(ErrorMessage = "Не задан режиссер!")]
        public string? director { get; set; }

        public int votes { get; set; }


        public override string ToString()
        {
            return $"Serial {id} : {title}, {rating}, {year}, {age_limit}+, {director}\n" +
                $"description: {description}";
        }
    }
}
