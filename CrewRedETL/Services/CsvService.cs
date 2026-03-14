using CrewRedETL.Models;
using CsvHelper;

public class CsvService
{
    private static readonly TimeZoneInfo EasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public IEnumerable<TripData> ProcessRecords(IEnumerable<TripData> rawRecords, CsvWriter duplicatesWriter, HashSet<string> seenKeys)
    {
        foreach (var record in rawRecords)
        {
            if (record.PickupDatetime == null || record.DropoffDatetime == null) continue;

            record.StoreAndFwdFlag = record.StoreAndFwdFlag?.ToUpper() switch
            {
                "N" => "No",
                "Y" => "Yes",
                _ => record.StoreAndFwdFlag
            };

            var key = $"{record.PickupDatetime.Value:yyyyMMddHHmmss}|{record.DropoffDatetime.Value:yyyyMMddHHmmss}|{record.PassengerCount}";

            if (!seenKeys.Add(key))
            {
                duplicatesWriter.WriteRecord(record);
                duplicatesWriter.NextRecord();
                continue;
            }

            record.PickupDatetime = TimeZoneInfo.ConvertTimeToUtc(record.PickupDatetime.Value, EasternZone);
            record.DropoffDatetime = TimeZoneInfo.ConvertTimeToUtc(record.DropoffDatetime.Value, EasternZone);

            yield return record;
        }
    }
}