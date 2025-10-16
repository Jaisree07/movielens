using MovieLensApp.Controller;
using System;

namespace MovieLensApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AppController app = new();
            app.Run();
            Console.WriteLine("Reports generated successfully.");
            Console.ReadLine();
        }
    }
}
