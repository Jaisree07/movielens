using System;

namespace View
{
    public class ConsoleInterface
    {
        public string AskDatasetFolder()
        {   
            return @"C:\Users\jaisree.anandhakumar\Downloads\ml-100k\movielens\Dataset";
        }
        public bool AskUseMultithreading()
        {
            Console.Write("Use multithreading? (y/n): ");
            string answer = Console.ReadLine()!.ToLower();
            return answer == "y" || answer == "yes";
        }
        public void ShowMessage(string message) => Console.WriteLine(message);
    }
}
