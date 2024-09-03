using MoviesAPI.Models;

namespace MoviesAPI.Models
{
	public class Movies_Model
	{
		public long Id { get; set; }
		public decimal Amount { get; set; }
		public int Duration { get; set; }
		public string? Name { get; set; }
		public DateTime Release_date { get; set; }
		public float Price { get; set; }
		public decimal Rating { get; set; }
		public string Details { get; set; }
		public string Quality { get; set; }
        public string Genre { get; set; }
        public byte[]? Image { get; set; }
        public string? Ytlink { get; set; }



    }
    public class Create_Movies_Model
	{
	//	public decimal Amount { get; set; }
		public int Duration { get; set; }
		public string? Name { get; set; }
		public DateTime Release_date { get; set; }
		public float Price { get; set; }
		public decimal Rating { get; set; }
		public string Details { get; set; }
		public string Quality { get; set; }
        public string Genre { get; set; }
        public byte[]? Image { get; set; }
        public string? Ytlink { get; set; }




    }
    public class Update_Movie_Model
	{
		public decimal? Amount { get; set; }
		public int? Duration { get; set; }
		public string? Name { get; set; }
		public DateTime? Release_date { get; set; }
		public float? Price { get; set; }
		public decimal? Rating { get; set; }
		public string? Details { get; set; }
		public string? Quality { get; set; }
        public string? Genre { get; set; }
        public byte[]? Image { get; set; }
        public string? Ytlink { get; set; }




    }



    public class MovieViewModel
        {
            public Movies_Model Movie { get; set; }
            public bool IsPurchased { get; set; }
		    public List<Comments_Model> Comments { get; set; }
			public int UserId { get; set; }
        public int RatingMovie { get; set; }

        public int TotalUsersRated { get; set; }
        public double AverageRating { get; set; }

        public double averageRatingMultiply2 { get; set; }



    }


}
// price,rating,details,quality

