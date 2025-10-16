using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MovieLensApp.Model
{
    public class MovieLensData
    {
        public List<Movie> Movies { get; private set; } = new();
        public List<User> Users { get; private set; } = new();
        public List<Rating> Ratings { get; private set; } = new();

        private readonly string datasetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dataset");

        private readonly string[] GenreNames = new string[]
        {
            "Unknown", "Action", "Adventure", "Animation", "Children's", "Comedy",
            "Crime", "Documentary", "Drama", "Fantasy", "Film-Noir", "Horror",
            "Musical", "Mystery", "Romance", "Sci-Fi", "Thriller", "War", "Western"
        };

        public void LoadData()
        {
            LoadMovies();
            LoadUsers();
            LoadRatings();
        }

        private void LoadMovies()
        {
            string file = Path.Combine(datasetFolder, "u.item");
            foreach (var line in File.ReadAllLines(file))
            {
                var parts = line.Split('|');
                var genreFlags = parts.Skip(5).Take(19).Select(int.Parse).ToArray();

                var genres = new List<string>();
                for (int i = 0; i < genreFlags.Length; i++)
                    if (genreFlags[i] == 1) genres.Add(GenreNames[i]);

                Movies.Add(new Movie
                {
                    MovieId = int.Parse(parts[0]),
                    Title = parts[1],
                    GenreList = genres
                });
            }
        }

        private void LoadUsers()
        {
            string file = Path.Combine(datasetFolder, "u.user");
            foreach (var line in File.ReadAllLines(file))
            {
                var parts = line.Split('|');
                Users.Add(new User
                {
                    UserId = int.Parse(parts[0]),
                    Age = int.Parse(parts[1]),
                    Gender = parts[2],
                    Occupation = parts[3],
                    ZipCode = parts[4]
                });
            }
        }

        private void LoadRatings()
        {
            string file = Path.Combine(datasetFolder, "u.data");
            foreach (var line in File.ReadAllLines(file))
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
    }
}
