using MovieFinder.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MovieFinder.Models
{
    public class Movie
    {
        public int id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Русское название не задано")]
        public string russian_name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Оригинальное название не задано")]
        public string original_name { get; set; }


        public string image_url { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг должен быть в диапазоне от 0 до 5")]
        public double rating { get; set; }

        [YearRange(ErrorMessage = "Некорректный год")]
        public int year { get; set; }

        [Range(10, int.MaxValue, ErrorMessage = "Некорректная длина фильма")]
        public int duration { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Описание не задано")]
        public string description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Краткое описание не задано")]
        public string short_description { get; set; }

        [Range(0, 18, ErrorMessage = "Некорректный формат на возрастное ограничение(0-18)")]
        public int age_limit { get; set; }


        public string countries { get; set; }
        public string genres { get; set; }

        [Required(ErrorMessage = "Режиссер не задан")]
        public string director { get; set; }


        public int votes { get; set; }

        public override string ToString()
        {
            return $"Movie {id} : {russian_name}, {rating}, {year}, {age_limit}+, {director}\n" +
                $"description: {description}";
        }

    }

}
