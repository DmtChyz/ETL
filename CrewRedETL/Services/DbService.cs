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
            }

            using var bulk = new SqlBulkCopy(_connectionString);
            bulk.DestinationTableName = "TripData";

            foreach (DataColumn col in dt.Columns) bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            bulk.WriteToServer(dt);
        }
    }
}
