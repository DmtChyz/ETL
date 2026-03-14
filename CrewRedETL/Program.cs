using CrewRedETL.Data;
using CrewRedETL.Models;
using CrewRedETL.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

internal class Program
{
    static void Main(string[] args)
    {
        var connectionString = "Server=localhost;Database=CrewRedDb;Trusted_Connection=True;TrustServerCertificate=True;";
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        var dbService = new DbService(connectionString);
        var csvService = new CsvService();
        var queryService = new TripQueryService(connectionString);

        using (var dbContext = new AppDbContext(optionsBuilder.Options))
        {
            dbContext.Database.EnsureCreated();

            if (!dbContext.TripData.Any())
            {
                var seenKeys = new HashSet<string>();
                var batch = new List<TripData>();

                using var dupWriter = new StreamWriter("duplicates.csv");
                using var dupCsv = new CsvWriter(dupWriter, CultureInfo.InvariantCulture);
                using var reader = new StreamReader("sample-cab-data.csv");
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { TrimOptions = TrimOptions.Trim });

                csv.Context.RegisterClassMap<TripDataMap>();

                foreach (var record in csvService.ProcessRecords(csv.GetRecords<TripData>(), dupCsv, seenKeys))
                {
                    batch.Add(record);
                    if (batch.Count >= 5000)
                    {
                        dbService.BulkInsert(batch);
                        batch.Clear();
                    }
                }

                if (batch.Count > 0) dbService.BulkInsert(batch);
                Console.WriteLine("Import complete!");
            }
            else
            {
                Console.WriteLine($"Table already has {dbContext.TripData.Count()} rows, skipping import.");
            }
        }

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
                    var tip = queryService.GetHighestAverageTip();
                    Console.WriteLine($"Top Location: {tip?.PULocationId}, Avg Tip: {tip?.AverageTipAmount:C}");
                    break;
                case "2":
                    var dist = queryService.GetTop100LongestByTripDistance();
                    dist.ForEach(t => Console.WriteLine($"Dist: {t.TripDistance}, ID: {t.PULocationId}"));
                    break;
                case "3":
                    var time = queryService.GetTop100LongestByTravelTime();
                    time.ForEach(t => Console.WriteLine($"Time: {t.TravelTimeSeconds}s, ID: {t.PULocationId}"));
                    break;
                case "4":
                    Console.Write("Enter PULocationID: ");
                    int id = int.Parse(Console.ReadLine());
                    var results = queryService.Search(id);
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