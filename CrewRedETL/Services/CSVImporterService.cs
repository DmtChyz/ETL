using CrewRedETL.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewRedETL.Services
{
    public class CSVImporterService
    {
        private readonly CsvService _csvService;
        private readonly DbService _dbService;

        public CSVImporterService(CsvService csvService, DbService dbService)
        {
            _csvService = csvService;
            _dbService = dbService;
        }

        public void Import(string inputPath, string duplicatesPath)
        {
            var seenKeys = new HashSet<string>();
            var batch = new List<TripData>();

            using var dupWriter = new StreamWriter(duplicatesPath);
            using var dupCsv = new CsvWriter(dupWriter, CultureInfo.InvariantCulture);
            using var csv = CreateCsvReader(inputPath);

            foreach (var record in _csvService.ProcessRecords(csv.GetRecords<TripData>(), dupCsv, seenKeys))
            {
                batch.Add(record);
                if (batch.Count >= 5000) FlushBatch(batch);
            }

            FlushBatch(batch);
            Console.WriteLine("Import complete!");
        }

        private void FlushBatch(List<TripData> batch)
        {
            if (batch.Count == 0) return;
            _dbService.BulkInsert(batch);
            batch.Clear();
        }
        

        private CsvReader CreateCsvReader(string inputPath)
        {
            var reader = new StreamReader(inputPath);
            var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            { TrimOptions = TrimOptions.Trim });
            csv.Context.RegisterClassMap<TripDataMap>();
            return csv;
        }
    }
}
