namespace Movie.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string[] Genres { get; set; }

        public override string ToString() => $"{MovieId}: {Title}";
    }
}
