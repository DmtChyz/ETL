using CrewRedETL.Data;
using CrewRedETL.Services;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    static void Main(string[] args)
    {
        var connectionString = "Server=localhost;Database=CrewRedDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

        var dbService = new DbService(connectionString);
        var csvService = new CsvService();

        using (var dbContext = new AppDbContext(optionsBuilder.Options))
        {
            dbContext.Database.EnsureCreated();

            // empty? -> Import
            if (!dbContext.TripData.Any())
            {
                var importer = new CSVImporterService(csvService, dbService);
                importer.Import("sample-cab-data.csv", "duplicates.csv");
            }
            else // more than one? -> Leave it
            {
                Console.WriteLine($"Table already has {dbContext.TripData.Count()} rows, skipping import.");
            }
        }

        var queryService = new TripQueryService(connectionString);
        var menu = new MenuService(queryService);
        menu.Run();
    }
}