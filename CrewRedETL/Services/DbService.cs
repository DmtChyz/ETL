using CrewRedETL.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewRedETL.Services
{
    public class DbService
    {
        private readonly string _connectionString;

        public DbService(string connectionString) => _connectionString = connectionString;

        public void BulkInsert(List<TripData> data)
        {
            using var dt = new DataTable();
            dt.Columns.Add("PickupDatetime", typeof(DateTime));
            dt.Columns.Add("DropoffDatetime", typeof(DateTime));
            dt.Columns.Add("PassengerCount", typeof(byte));
            dt.Columns.Add("TripDistance", typeof(decimal));
            dt.Columns.Add("StoreAndFwdFlag", typeof(string));
            dt.Columns.Add("PULocationId", typeof(short));
            dt.Columns.Add("DOLocationId", typeof(short));
            dt.Columns.Add("FareAmount", typeof(decimal));
            dt.Columns.Add("TipAmount", typeof(decimal));

            foreach (var row in data)
            {
                // with converting into default value
                dt.Rows.Add(
                    row.PickupDatetime,
                    row.DropoffDatetime, 
                    (byte)(row.PassengerCount ?? 0),
                    (decimal)(row.TripDistance ?? 0), 
                    row.StoreAndFwdFlag,
                    (short)(row.PULocationId ?? 0),
                    (short)(row.DOLocationId ?? 0),
                    row.FareAmount,
                    row.TipAmount);
                // with nulls
                //dt.Rows.Add(
                //    row.PickupDatetime ?? (object)DBNull.Value,
                //    row.DropoffDatetime ?? (object)DBNull.Value,
                //    row.PassengerCount.HasValue ? (object)(byte)row.PassengerCount.Value : DBNull.Value,
                //    row.TripDistance.HasValue ? (object)(decimal)row.TripDistance.Value : DBNull.Value,
                //    row.StoreAndFwdFlag ?? (object)DBNull.Value,
                //    row.PULocationId.HasValue ? (object)(short)row.PULocationId.Value : DBNull.Value,
                //    row.DOLocationId.HasValue ? (object)(short)row.DOLocationId.Value : DBNull.Value,
                //    row.FareAmount ?? (object)DBNull.Value,
                //    row.TipAmount ?? (object)DBNull.Value);
            }

            using var bulk = new SqlBulkCopy(_connectionString);
            bulk.DestinationTableName = "TripData";

            foreach (DataColumn col in dt.Columns) bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            bulk.WriteToServer(dt);
        }
    }
}
