namespace MoviesAPI.Models
{
    public class Rating_Movie_Model
    {
        public long Id { get; set; }
        public long User_id { get; set; }
        public int Movie_id { get; set; }
        public int Rating { get; set; }
    }

    public class Create_Rating_Movie_Model
    {
        public long User_id { get; set; }
        public int Movie_id { get; set; }
        public int Rating { get; set; }
    }
    public class AllMoviesRatingAvg
    {
        public long Movie_id { get; set; }
        public decimal average_rating { get; set; }
    }

}
