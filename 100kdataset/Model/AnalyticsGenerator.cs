using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Model
{
    public class AnalyticsGenerator
    {
        private readonly DatasetLoader _data;
        private readonly string _outputPath;

        public AnalyticsGenerator(DatasetLoader data, string outputPath)
        {
            _data = data;
            _outputPath = outputPath;
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);
        }

        public void GenerateReports(bool useMultithreading)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (useMultithreading)
                GenerateWithThreads();
            else
                GenerateWithoutThreads();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.Elapsed.TotalSeconds:F2} seconds");
        }
        private void GenerateWithoutThreads()
        {
            string folder = Path.Combine(_outputPath, "withoutMultithreading");
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            GenerateAllReports(_data.Ratings, folder);
        }
        private void GenerateWithThreads()
        {
            string folder = Path.Combine(_outputPath, "withMultithreading");
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            int chunkSize = 10000;
            int total = _data.Ratings.Count;
            int threadCount = (int)Math.Ceiling((double)total / chunkSize);
            var partialRatings = new ConcurrentBag<List<Rating>>();
            var tasks = new List<Task>();
            for (int t = 0; t < threadCount; t++)
            {
                int start = t * chunkSize;
                int end = Math.Min(start + chunkSize, total);

                tasks.Add(Task.Run(() =>
                {
                    var chunk = _data.Ratings.GetRange(start, end - start);
                    partialRatings.Add(chunk);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            var mergedRatings = partialRatings.SelectMany(c => c).ToList();
            GenerateAllReports(mergedRatings, folder);
        }

        private void GenerateAllReports(List<Rating> ratings, string folder)
        {
            Directory.CreateDirectory(folder);

            SaveCsv(Top10Movies(ratings), Path.Combine(folder, "Top10_General.csv"));
            SaveCsv(Top10MoviesByGender(ratings, "M"), Path.Combine(folder, "Top10_Male.csv"));
            SaveCsv(Top10MoviesByGender(ratings, "F"), Path.Combine(folder, "Top10_Female.csv"));
            SaveCsv(Top10MoviesByGenre(ratings, "Action"), Path.Combine(folder, "Top10_Action.csv"));
            SaveCsv(Top10MoviesByGenre(ratings, "Drama"), Path.Combine(folder, "Top10_Drama.csv"));
            SaveCsv(Top10MoviesByGenre(ratings, "Comedy"), Path.Combine(folder, "Top10_Comedy.csv"));
            SaveCsv(Top10MoviesByGenre(ratings, "Fantasy"), Path.Combine(folder, "Top10_Fantasy.csv"));
            SaveCsv(Top10MoviesByAgeRange(ratings, 0, 17), Path.Combine(folder, "Top10_Under18.csv"));
            SaveCsv(Top10MoviesByAgeRange(ratings, 18, 30), Path.Combine(folder, "Top10_18to30.csv"));
            SaveCsv(Top10MoviesByAgeRange(ratings, 31, 120), Path.Combine(folder, "Top10_Above30.csv"));
        }

        private List<(string Title, double Avg)> Top10Movies(List<Rating> ratings)
        {
            return ratings
                .GroupBy(r => r.MovieId)
                .Select(g => (Title: _data.Movies[g.Key].Title, Avg: g.Average(r => r.Score)))
                .OrderByDescending(x => x.Avg)
                .Take(10)
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByGender(List<Rating> ratings, string gender)
        {
            var users = _data.Users.Values.Where(u => u.Gender == gender)
                .Select(u => u.Id).ToHashSet();
            return ratings
                .Where(r => users.Contains(r.UserId))
                .GroupBy(r => r.MovieId)
                .Select(g => (Title: _data.Movies[g.Key].Title, Avg: g.Average(r => r.Score)))
                .OrderByDescending(x => x.Avg)
                .Take(10)
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByGenre(List<Rating> ratings, string genre)
        {
            var movieIds = _data.Movies.Values.Where(m => m.Genres.Contains(genre))
                .Select(m => m.Id).ToHashSet();
            return ratings
                .Where(r => movieIds.Contains(r.MovieId))
                .GroupBy(r => r.MovieId)
                .Select(g => (Title: _data.Movies[g.Key].Title, Avg: g.Average(r => r.Score)))
                .OrderByDescending(x => x.Avg)
                .Take(10)
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByAgeRange(List<Rating> ratings, int minAge, int maxAge)
        {
            var users = _data.Users.Values.Where(u => u.Age >= minAge && u.Age <= maxAge)
                .Select(u => u.Id).ToHashSet();
            return ratings
                .Where(r => users.Contains(r.UserId))
                .GroupBy(r => r.MovieId)
                .Select(g => (Title: _data.Movies[g.Key].Title, Avg: g.Average(r => r.Score)))
                .OrderByDescending(x => x.Avg)
                .Take(10)
                .ToList();
        }

        private void SaveCsv(List<(string Title, double Avg)> data, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var writer = new StreamWriter(path);
            writer.WriteLine("Title,AverageRating");
            foreach (var d in data)
                writer.WriteLine($"{d.Title},{d.Avg:F2}");
        }
    }
}
