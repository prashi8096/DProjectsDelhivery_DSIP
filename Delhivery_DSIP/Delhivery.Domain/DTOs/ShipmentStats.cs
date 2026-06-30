namespace Delhivery.Domain.Entities
{
    public class ShipmentStats
    {
        public int Booked { get; set; }

        public int InTransit { get; set; }

        public int OutForDelivery { get; set; }

        public int Delivered { get; set; }

        public int RTO { get; set; }
    }
}