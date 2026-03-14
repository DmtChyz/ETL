namespace CrewRedETL.Models
{
    public class TripDataEntity
    {
        public int Id { get; set; }
        public DateTime? PickupDatetime { get; set; }
        public DateTime? DropoffDatetime { get; set; }
        public int? PassengerCount { get; set; }
        public double? TripDistance { get; set; }
        public string? StoreAndFwdFlag { get; set; }
        public int? PULocationId { get; set; }
        public int? DOLocationId { get; set; }
        public decimal? FareAmount { get; set; }
        public decimal? TipAmount { get; set; }
        public int? TravelTimeSeconds { get; set; }
    }
}