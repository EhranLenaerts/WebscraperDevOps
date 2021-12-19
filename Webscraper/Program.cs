using System;
namespace MainWebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            //Asking what part of the program the user would like
            Console.WriteLine("What would you like to scrape? Press the associated number for your choice.");
            Console.WriteLine("Youtube : 1");
            Console.WriteLine("be.indeed : 2");
            Console.WriteLine("Twitter : 3");
            Console.WriteLine("(Numbers only)");
            //Using a switch for the different options
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    WebScraping.Youtube.Yt();
                    break;
                case 2:
                    WebScraping2.IndeedJob.Job();
                    break;
                case 3:
                    WebScraping3.Twitter.Tweets();
                    break;
            }

        }
    }
}