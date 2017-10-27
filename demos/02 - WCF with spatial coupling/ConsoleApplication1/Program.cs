using System;
using ConsoleApplication1.ServiceReference1;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press key to start");
            Console.ReadKey();

            var gameReviewMessage = new TvShowReviewMessage()
            {
                Name = "Game of Thrones",
                Review = "It is awesome!"
            };
            
            var svc = new Service1Client();
            string result = svc.AddTvShowReview(gameReviewMessage);

            Console.WriteLine("Result is : {0}", result);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
