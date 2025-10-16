using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model
{
    public class DatasetLoader
    {
        public Dictionary<int, Movie> Movies { get; } = new();
        public Dictionary<int, User> Users { get; } = new();
        public List<Rating> Ratings { get; } = new();
        public void LoadData(string folderPath)
        {
            foreach (var line in File.ReadAllLines(Path.Combine(folderPath, "u.item")))
            {
                var parts = line.Split('|');
                int id = int.Parse(parts[0]);
                string title = parts[1];
                var genres = parts.Skip(5)
                                  .Select((g, i) => g == "1" ? GenreIndexToName(i) : null)
                                  .Where(x => x != null).ToList()!;
                Movies[id] = new Movie { Id = id, Title = title, Genres = genres };
            }
            foreach (var line in File.ReadAllLines(Path.Combine(folderPath, "u.user")))
            {
                var parts = line.Split('|');
                Users[int.Parse(parts[0])] = new User
                {
                    Id = int.Parse(parts[0]),
                    Age = int.Parse(parts[1]),
                    Gender = parts[2],
                    Occupation = parts[3]
                };
            }
            foreach (var line in File.ReadAllLines(Path.Combine(folderPath, "u.data")))
            {
                var parts = line.Split('\t');
                Ratings.Add(new Rating
                {
                    UserId = int.Parse(parts[0]),
                    MovieId = int.Parse(parts[1]),
                    Score = int.Parse(parts[2]),
                    Timestamp = long.Parse(parts[3])
                });
            }
        }
        private string GenreIndexToName(int index)
        {
            string[] genres = { "Unknown", "Action", "Adventure", "Animation", "Children", "Comedy",
                                "Crime", "Documentary", "Drama", "Fantasy", "Film-Noir", "Horror",
                                "Musical", "Mystery", "Romance", "Sci-Fi", "Thriller", "War", "Western" };
            return index < genres.Length ? genres[index] : "Unknown";
        }
    }
}
