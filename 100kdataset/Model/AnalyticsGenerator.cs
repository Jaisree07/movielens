using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLensApp.Model
{
    public class AnalyticsGenerator
    {
        private readonly MovieLensData _data;
        private readonly string _outputPath;

        public AnalyticsGenerator(MovieLensData data)
        {
            _data = data;
            _outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);
        }

        public void GenerateReports(bool useThreads)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string folderName = useThreads ? "WithMultithreading" : "WithoutMultithreading";
            string folder = Path.Combine(_outputPath, folderName);
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            List<Rating> ratingsToUse;

            if (useThreads)
                ratingsToUse = ProcessWithThreads();
            else
                ratingsToUse = _data.Ratings;

            GenerateAllReports(ratingsToUse, folder);

            watch.Stop();
            Console.WriteLine($"Execution Time ({folderName}): {watch.Elapsed.TotalSeconds:F2} seconds");
        }

        private List<Rating> ProcessWithThreads()
        {
            int chunkSize = 10000;
            int total = _data.Ratings.Count;
            int threadCount = (int)Math.Ceiling((double)total / chunkSize);

            var bag = new ConcurrentBag<List<Rating>>();
            var tasks = new List<Task>();

            for (int t = 0; t < threadCount; t++)
            {
                int start = t * chunkSize;
                int end = Math.Min(start + chunkSize, total);

                tasks.Add(Task.Run(() =>
                {
                    var chunk = _data.Ratings.GetRange(start, end - start);
                    bag.Add(chunk);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            return bag.SelectMany(x => x).ToList();
        }

        private void GenerateAllReports(List<Rating> ratings, string folder)
        {
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
            var movieDict = _data.Movies.ToDictionary(m => m.MovieId, m => m.Title);

            return ratings.GroupBy(r => r.MovieId)
                .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Score), Count = g.Count() })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .ThenBy(x => x.MovieId)
                .Take(10)
                .Select(x => (Title: movieDict[x.MovieId], Avg: x.Avg))
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByGender(List<Rating> ratings, string gender)
        {
            var userIds = _data.Users.Where(u => u.Gender == gender).Select(u => u.UserId).ToHashSet();
            var movieDict = _data.Movies.ToDictionary(m => m.MovieId, m => m.Title);

            return ratings.Where(r => userIds.Contains(r.UserId))
                .GroupBy(r => r.MovieId)
                .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Score), Count = g.Count() })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .ThenBy(x => x.MovieId)
                .Take(10)
                .Select(x => (Title: movieDict[x.MovieId], Avg: x.Avg))
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByGenre(List<Rating> ratings, string genre)
        {
            var movieIds = _data.Movies.Where(m => m.GenreList.Contains(genre)).Select(m => m.MovieId).ToHashSet();
            var movieDict = _data.Movies.ToDictionary(m => m.MovieId, m => m.Title);

            return ratings.Where(r => movieIds.Contains(r.MovieId))
                .GroupBy(r => r.MovieId)
                .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Score), Count = g.Count() })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .ThenBy(x => x.MovieId)
                .Take(10)
                .Select(x => (Title: movieDict[x.MovieId], Avg: x.Avg))
                .ToList();
        }

        private List<(string Title, double Avg)> Top10MoviesByAgeRange(List<Rating> ratings, int minAge, int maxAge)
        {
            var userIds = _data.Users.Where(u => u.Age >= minAge && u.Age <= maxAge).Select(u => u.UserId).ToHashSet();
            var movieDict = _data.Movies.ToDictionary(m => m.MovieId, m => m.Title);

            return ratings.Where(r => userIds.Contains(r.UserId))
                .GroupBy(r => r.MovieId)
                .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Score), Count = g.Count() })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .ThenBy(x => x.MovieId)
                .Take(10)
                .Select(x => (Title: movieDict[x.MovieId], Avg: x.Avg))
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
