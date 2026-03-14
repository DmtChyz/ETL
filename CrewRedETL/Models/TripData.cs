using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewRedETL.Models
{
    public class TripData
    { 
        public DateTime? PickupDatetime { get; set; }
        public DateTime? DropoffDatetime { get; set; }
        public int? PassengerCount { get; set; }
        public double? TripDistance { get; set; }
        public string? StoreAndFwdFlag { get; set; }
        public int? PULocationId { get; set; }
        public int? DOLocationId { get; set; }
        public decimal? FareAmount { get; set; }
        public decimal? TipAmount { get; set; }
    }
}
