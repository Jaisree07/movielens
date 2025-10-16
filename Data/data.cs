using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Movie.Models;

namespace Movie.Data
{
    public class MovieDataLoader
    {
        private string dataPath;

        public MovieDataLoader(string path)
        {
            dataPath = path;
        }

        public List<Movie> LoadMovies()
        {
            var movies = new List<Movie>();
            foreach (var line in File.ReadLines(Path.Combine(dataPath, "u.item")))
            {
                var parts = line.Split('|');
                if (parts.Length < 24) continue;
                var genres = parts.Skip(5).Select((g, i) => g == "1" ? GenreName(i) : null)
                                  .Where(g => g != null).ToArray();
                movies.Add(new Movie
                {
                    MovieId = int.Parse(parts[0]),
                    Title = parts[1],
                    Genres = genres
                });
            }
            return movies;
        }

        public List<User> LoadUsers()
        {
            var users = new List<User>();
            foreach (var line in File.ReadLines(Path.Combine(dataPath, "u.user")))
            {
                var parts = line.Split('|');
                users.Add(new User
                {
                    UserId = int.Parse(parts[0]),
                    Age = int.Parse(parts[1]),
                    Gender = parts[2],
                    Occupation = parts[3],
                    ZipCode = parts[4]
                });
            }
            return users;
        }

        public List<Rating> LoadRatings()
        {
            var ratings = new List<Rating>();
            foreach (var line in File.ReadLines(Path.Combine(dataPath, "u.data")))
            {
                var parts = line.Split('\t');
                ratings.Add(new Rating
                {
                    UserId = int.Parse(parts[0]),
                    MovieId = int.Parse(parts[1]),
                    Score = double.Parse(parts[2]),
                    Timestamp = long.Parse(parts[3])
                });
            }
            return ratings;
        }

        private string GenreName(int index)
        {
            string[] genres = {
                "Unknown","Action","Adventure","Animation","Children’s","Comedy","Crime","Documentary","Drama","Fantasy","Film-Noir","Horror","Musical","Mystery","Romance","Sci-Fi","Thriller","War","Western"
            };
            return index < genres.Length ? genres[index] : "Unknown";
        }
    }
}
