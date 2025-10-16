using System;
using Movie.Data;
using Movie.Controllers;
using Movie.Views;

namespace Movie
{
    class Program
    {
        static void Main()
        {
            string datasetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            var loader = new MovieDataLoader(datasetPath);
            var movies = loader.LoadMovies();
            var users = loader.LoadUsers();
            var ratings = loader.LoadRatings();

            var controller = new ReportController(movies, users, ratings);
            var view = new MenuView(controller);
            view.ShowMenu();
        }
    }
}
