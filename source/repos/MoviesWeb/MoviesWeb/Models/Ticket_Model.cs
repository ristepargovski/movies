namespace MoviesAPI.Models
{
    public class Ticket_Model
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public int Movie_id { get; set; }
        public int User_id { get; set; }
        public DateTime Watch_movie { get; set; }
    }

    public class Create_Ticket_Model
    {
        public decimal Amount { get; set; }
        public int Movie_id { get; set; }
        public int User_id { get; set; }
        public int Tvshow_id { get; set; }
        public DateTime Watch_movie { get; set; }
    }
    public class Update_Ticket_Model
    {
        public decimal Amount { get; set; }
        public int Movie_id { get; set; }
        public int User_id { get; set; }
        public DateTime Watch_movie { get; set; }
    }
}
public class TicketWithMovieDetails
{
    // Ticket properties
    public long TicketId { get; set; }
    public decimal TicketAmount { get; set; }
    public int TicketMovieId { get; set; }
    public int UserId { get; set; }
    public DateTime WatchMovie { get; set; }

    // Movie properties
    public long MovieId { get; set; }
    public decimal MovieAmount { get; set; }
    public int Duration { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
    public float Price { get; set; }
    public decimal Rating { get; set; }
    public string Details { get; set; }
    public string Quality { get; set; }
    public string Genre { get; set; }
    public byte[]? Image { get; set; }
}


public class TicketWithTvShowDetails
{
    // Ticket properties
    public long TicketId { get; set; }
    public decimal TicketAmount { get; set; }
    public int TvshowId { get; set; }
    public int UserId { get; set; }
    public DateTime WatchTvShow { get; set; }

    // TV Show properties
    public string Name { get; set; }
    public int Seasons { get; set; }
    public string Episodes { get; set; }
    public float Price { get; set; }
    public decimal Rating { get; set; }
    public string Details { get; set; }
    public string Quality { get; set; }
    public string Genre { get; set; }
    public byte[]? Image { get; set; }
    public string Ytlink { get; set; }
    public DateTime ReleaseDate { get; set; }
}


public class TicketResponse
{
    public TicketWithMovieDetails? Movie { get; set; }
    public TicketWithTvShowDetails? Tvshow { get; set; }
}
public class TicketApiResponse
{
    public List<TicketWithMovieDetails>? Movies { get; set; }
    public List<TicketWithTvShowDetails>? TVShows { get; set; }
}
