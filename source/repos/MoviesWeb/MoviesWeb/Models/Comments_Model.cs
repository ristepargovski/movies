namespace MoviesAPI.Models
{
    public class Comments_Model
    {
        public long Id { get; set; }

        public string User_name { get; set; }
        public int User_id { get; set; }
        public int Movie_id { get; set; }
        public int Tvshow_id { get; set; }
        public string Comment { get; set; }
    }
    public class Create_Comments_Model
    {
        public int User_id { get; set; }
        public int Movie_id { get; set; }
        public int Tvshow_id { get; set; }
        public string Comment { get; set; }
    }
    public class Update_Comments_Model
    {
        public int User_id { get; set; }
        public int Movie_id { get; set; }
        public int Tvshow_id { get; set; }
        public string Comment { get; set; }
    }

}
