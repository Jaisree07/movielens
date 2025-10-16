namespace Movie.Models
{
    public class Rating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public double Score { get; set; }
        public long Timestamp { get; set; }
    }
}
