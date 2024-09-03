using MoviesAPI.Models;

namespace MoviesWeb.Models
{
    public class Tvshows_Model
    {


        public long Id { get; set; }
        public string? Name { get; set; }
        public int? Seasons { get; set; }
        public string? Episodes { get; set; }
        public float Price { get; set; }
        public decimal Rating { get; set; }
        public string Details { get; set; }
        public string Quality { get; set; }
        public string Genre { get; set; }
        public byte[]? Image { get; set; }
        public string Ytlink { get; set; }
        public DateTime Release_date { get; set; }




    }
    public class Create_Tvshows_Model
    {
        public string? Name { get; set; }
        public int? Seasons { get; set; }
        public string? Episodes { get; set; }
        public float Price { get; set; }
        public decimal Rating { get; set; }
        public string Details { get; set; }
        public string Quality { get; set; }
        public string Genre { get; set; }
        public byte[]? Image { get; set; }
        public string? Ytlink { get; set; }
        public DateTime Release_date { get; set; }

    }
    public class Update_Tvshows_Model
    {
        public string? Name { get; set; }
        public int? Seasons { get; set; }
        public string? Episodes { get; set; }
        public float? Price { get; set; }
        public decimal? Rating { get; set; }
        public string? Details { get; set; }
        public string? Quality { get; set; }
        public string? Genre { get; set; }
        public byte[]? Image { get; set; }
        public string? Ytlink { get; set; }
        public DateTime? Release_date { get; set; }

    }


    public class TvshowsViewModel
    {
        public Tvshows_Model Tvshows { get; set; }
        public bool IsPurchased { get; set; }
        public int RatingTvshow { get; set; }
        public List<Comments_Model> Comments { get; set; }
        public int UserId { get; set; }


        public int TotalUsersRated { get; set; }
        public double AverageRating { get; set; }

        public double averageRatingMultiply2 { get; set;}

    }
}
