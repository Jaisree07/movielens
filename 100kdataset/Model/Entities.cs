using System.Collections.Generic;

namespace MovieLensApp.Model
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public List<string> GenreList { get; set; } = new();
    }

    public class User
    {
        public int UserId { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string ZipCode { get; set; }
    }

    public class Rating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Score { get; set; }
        public long Timestamp { get; set; }
    }
}
