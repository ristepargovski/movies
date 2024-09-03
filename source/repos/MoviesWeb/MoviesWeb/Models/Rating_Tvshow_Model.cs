namespace MoviesWeb.Models
{
    public class Rating_Tvshow_Model
    {

      
            public long Id { get; set; }
            public long User_id { get; set; }
            public int Tvshow_id { get; set; }
            public int Rating { get; set; }
      

      

    }
    public class Create_Rating_Tvshow_Model
    {
        public long User_id { get; set; }
        public int Tvshow_id { get; set; }
        public int Rating { get; set; }
    }
}
