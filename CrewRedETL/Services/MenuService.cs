namespace CrewRedETL.Services
{
    public class MenuService
    {
        private readonly TripQueryService _queryService;

        public MenuService(TripQueryService queryService)
        {
            _queryService = queryService;
        }

        public void Run()
        {
            Console.WriteLine("\nPress Enter to open Query Menu...");
            Console.ReadLine();

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("1. Get Location with Highest Average Tip");
                Console.WriteLine("2. Get Top 100 Longest Fares by Distance");
                Console.WriteLine("3. Get Top 100 Longest Fares by Travel Time");
                Console.WriteLine("4. Search (by Location ID)");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        var tip = _queryService.GetHighestAverageTip();
                        Console.WriteLine($"Top Location: {tip?.PULocationId}, Avg Tip: {tip?.AverageTipAmount:C}");
                        break;
                    case "2":
                        var dist = _queryService.GetTop100LongestByTripDistance();
                        dist.ForEach(t => Console.WriteLine($"Dist: {t.TripDistance}, ID: {t.PULocationId}"));
                        break;
                    case "3":
                        var time = _queryService.GetTop100LongestByTravelTime();
                        time.ForEach(t => Console.WriteLine($"Time: {t.TravelTimeSeconds}s, ID: {t.PULocationId}"));
                        break;
                    case "4":
                        Console.Write("Enter PULocationID: ");
                        int id = int.Parse(Console.ReadLine());
                        var results = _queryService.Search(id);
                        results.ForEach(t => Console.WriteLine($"Fare: {t.FareAmount}, Dist: {t.TripDistance}"));
                        break;
                    case "0":
                        exit = true;
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}