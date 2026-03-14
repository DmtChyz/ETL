using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewRedETL.Models
{
    public class TripDataMap : ClassMap<TripData>
    {
        public TripDataMap()
        {
            Map(m => m.PickupDatetime).Name("tpep_pickup_datetime");
            Map(m => m.DropoffDatetime).Name("tpep_dropoff_datetime");
            Map(m => m.PassengerCount).Name("passenger_count");
            Map(m => m.TripDistance).Name("trip_distance");
            Map(m => m.StoreAndFwdFlag).Name("store_and_fwd_flag")
                .Convert(r =>
                {
                    var val = r.Row.GetField("store_and_fwd_flag")?.Trim();
                    return 
                    val == "Y" ? "Yes" : val == "N"
                                ? "No" : val ?? null;
                });
            Map(m => m.PULocationId).Name("PULocationID");
            Map(m => m.DOLocationId).Name("DOLocationID");
            Map(m => m.FareAmount).Name("fare_amount");
            Map(m => m.TipAmount).Name("tip_amount");
        }
    }
}
