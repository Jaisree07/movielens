using System;
using Movie.Controllers;

namespace Movie.Views
{
    public class MenuView
    {
        private readonly ReportController _controller;

        public MenuView(ReportController controller)
        {
            _controller = controller;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("====================================");
                Console.WriteLine("🎬 MovieLens OLAP Analytics");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Generate Reports (Single Thread)");
                Console.WriteLine("2. Generate Reports (Multi Thread)");
                Console.WriteLine("3. Exit");
                Console.Write("Select option: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _controller.GenerateReports(false);
                        break;
                    case "2":
                        _controller.GenerateReports(true);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
