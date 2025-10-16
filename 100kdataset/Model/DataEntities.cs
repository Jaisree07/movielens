using System.Collections.Generic;

namespace Model
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public List<string> Genres { get; set; } = new List<string>();
    }
    public class User
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = "";
        public string Occupation { get; set; } = "";
    }
    public class Rating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Score { get; set; }
        public long Timestamp { get; set; }
    }
}
