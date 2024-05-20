namespace MovieFinder.Models
{
    public class FavouriteModel
    {
        public int id { get; set; }
        public string type { get; set; }
        public int production_id { get; set; }
        public int user_id { get; set; }
    }
}
