using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Movie.Models;

namespace Movie.Controllers
{
    public class ReportController
    {
        private List<Movie> _movies;
        private List<User> _users;
        private List<Rating> _ratings;

        public ReportController(List<Movie> movies, List<User> users, List<Rating> ratings)
        {
            _movies = movies;
            _users = users;
            _ratings = ratings;
        }

        public void GenerateReports(bool multithreaded)
        {
            var watch = Stopwatch.StartNew();
            string folder = multithreaded ? "MultiThreadingReports" : "ThreadingReports";
            Directory.CreateDirectory(folder);

            if (multithreaded)
                RunMultithreadedReports(folder);
            else
                RunSingleThreadReports(folder);

            watch.Stop();
            Console.WriteLine($"\nReports generated in {watch.ElapsedMilliseconds / 1000.0:F2} seconds\n");
        }

        private void RunSingleThreadReports(string folder)
        {
            GenerateTopMovies(folder, "Top10General.csv", _ratings);
            GenerateGenderBased(folder);
            GenerateGenreBased(folder);
            GenerateAgeBased(folder);
        }

        private void RunMultithreadedReports(string folder)
        {
            var threads = new List<Thread>
            {
                new Thread(() => GenerateTopMovies(folder, "Top10General.csv", _ratings)),
                new Thread(() => GenerateGenderBased(folder)),
                new Thread(() => GenerateGenreBased(folder)),
                new Thread(() => GenerateAgeBased(folder))
            };

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
        }

        private void GenerateTopMovies(string folder, string fileName, List<Rating> ratings)
        {
            var top = ratings.GroupBy(r => r.MovieId)
                             .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Score) })
                             .OrderByDescending(g => g.Avg)
                             .Take(10)
                             .ToList();

            WriteCsv(Path.Combine(folder, fileName), top);
        }

        private void GenerateGenderBased(string folder)
        {
            var maleRatings = _ratings.Where(r => _users.First(u => u.UserId == r.UserId).Gender == "M").ToList();
            var femaleRatings = _ratings.Where(r => _users.First(u => u.UserId == r.UserId).Gender == "F").ToList();
            GenerateTopMovies(folder, "Top10_Male.csv", maleRatings);
            GenerateTopMovies(folder, "Top10_Female.csv", femaleRatings);
        }

        private void GenerateGenreBased(string folder)
        {
            string[] targetGenres = { "Action", "Drama", "Comedy", "Fantasy" };
            foreach (var genre in targetGenres)
            {
                var genreMovies = _movies.Where(m => m.Genres.Contains(genre)).Select(m => m.MovieId).ToHashSet();
                var genreRatings = _ratings.Where(r => genreMovies.Contains(r.MovieId)).ToList();
                GenerateTopMovies(folder, $"Top10_{genre}.csv", genreRatings);
            }
        }

        private void GenerateAgeBased(string folder)
        {
            var under18 = _users.Where(u => u.Age < 18).Select(u => u.UserId).ToHashSet();
            var between18_30 = _users.Where(u => u.Age >= 18 && u.Age <= 30).Select(u => u.UserId).ToHashSet();
            var above30 = _users.Where(u => u.Age > 30).Select(u => u.UserId).ToHashSet();

            GenerateTopMovies(folder, "Top10_Age_Under18.csv", _ratings.Where(r => under18.Contains(r.UserId)).ToList());
            GenerateTopMovies(folder, "Top10_Age_18_30.csv", _ratings.Where(r => between18_30.Contains(r.UserId)).ToList());
            GenerateTopMovies(folder, "Top10_Age_Above30.csv", _ratings.Where(r => above30.Contains(r.UserId)).ToList());
        }

        private void WriteCsv(string filePath, IEnumerable<object> data)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("MovieId,AverageRating");
            foreach (var item in data)
                writer.WriteLine($"{item.GetType().GetProperty("MovieId")?.GetValue(item)},{item.GetType().GetProperty("Avg")?.GetValue(item)}");
        }
    }
}
