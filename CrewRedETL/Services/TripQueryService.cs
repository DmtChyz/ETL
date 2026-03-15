using CrewRedETL.Models;
using Microsoft.Data.SqlClient;
using System.Data;

public sealed class TripQueryService
{
    private readonly string _connectionString;

    public TripQueryService(string connectionString) => _connectionString = connectionString;

    public AverageTipResult GetHighestAverageTip()
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        using var cmd = new SqlCommand("" +
            "SELECT TOP 1 PULocationId, AVG(TipAmount) as AvgTip " +
            "FROM TripData " +
            "WHERE PULocationId IS NOT NULL " +
            "GROUP BY PULocationId " +
            "ORDER BY AvgTip DESC", conn);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null; // read - read row only once.
        return new AverageTipResult
        {   
            PULocationId = r.GetInt16(0),
            AverageTipAmount = r.GetDecimal(1)
        };
    }

    public List<TripDataEntity> GetTop100LongestByTripDistance() => ReadTrips("" +
        "SELECT TOP 100 * " +
        "FROM TripData " +
        "ORDER BY TripDistance DESC, Id DESC", null);

    public List<TripDataEntity> GetTop100LongestByTravelTime() => ReadTrips("" +
        "SELECT TOP 100 * " +
        "FROM TripData" +
        " WHERE travel_time_seconds IS NOT NULL" +
        " ORDER BY travel_time_seconds DESC, Id DESC", null);

    public List<TripDataEntity> Search(int locationId)
    {
        var sql = "SELECT TOP 20 * FROM TripData WHERE PULocationId = @id"; // placeholder parameter
        var param = new SqlParameter("@id", locationId);
        return ReadTrips(sql, new List<SqlParameter> { param });
    }

    private List<TripDataEntity> ReadTrips(string sql, List<SqlParameter> parameters)
    {
        var results = new List<TripDataEntity>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters.ToArray()); // for placeholder parameters like in Search method on top
        conn.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            results.Add(new TripDataEntity
            {   // maping data with its type based on sql
                Id = r.GetInt32(r.GetOrdinal("Id")),
                PickupDatetime = ReadNullableDateTime(r, "PickupDatetime"),
                DropoffDatetime = ReadNullableDateTime(r, "DropoffDatetime"),
                PassengerCount = ReadNullableInt32(r, "PassengerCount"),
                TripDistance = ReadNullableDouble(r, "TripDistance"),
                StoreAndFwdFlag = ReadNullableString(r, "StoreAndFwdFlag"),
                PULocationId = ReadNullableInt32(r, "PULocationId"),
                DOLocationId = ReadNullableInt32(r, "DOLocationId"),
                FareAmount = ReadNullableDecimal(r, "FareAmount"),
                TipAmount = ReadNullableDecimal(r, "TipAmount"),
                TravelTimeSeconds = ReadNullableInt32(r, "travel_time_seconds")
            });
        }
        return results;
    }

    private static DateTime? ReadNullableDateTime(SqlDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDateTime(r.GetOrdinal(col));
    private static int? ReadNullableInt32(SqlDataReader r, string col)
    {
        var ordinal = r.GetOrdinal(col);
        if (r.IsDBNull(ordinal)) return null;

        var type = r.GetFieldType(ordinal);

        if (type == typeof(byte))
            return (int)r.GetByte(ordinal);

        if (type == typeof(short))
            return (int)r.GetInt16(ordinal);

        return r.GetInt32(ordinal);
    }
    private static double? ReadNullableDouble(SqlDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : (double)r.GetDouble(r.GetOrdinal(col));
    private static decimal? ReadNullableDecimal(SqlDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDecimal(r.GetOrdinal(col));
    private static string ReadNullableString(SqlDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetString(r.GetOrdinal(col));
}