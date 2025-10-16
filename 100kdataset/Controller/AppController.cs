using System;

using MovieLensApp.Model;
using MovieLensApp.View;

namespace MovieLensApp.Controller
{
    public class AppController
    {
        private readonly MovieLensData _data;
        private readonly AnalyticsGenerator _analytics;
        public AppController()
        {
            _data = new MovieLensData();
            _data.LoadData();
            _analytics = new AnalyticsGenerator(_data);
        }
        public void Run()
        {
            ConsoleView.ShowMessage("Do you want to use multithreading? (y/n): ");
            string input = Console.ReadLine()?.ToLower() ?? "n";
            bool useThreads = input == "y";

            _analytics.GenerateReports(useThreads);
        }
    }
}
