using System;
using Delhivery.Domain.Enums;

namespace Delhivery.Domain.Entities
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public string AWBNumber { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public double WeightKg { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime BookedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
    }
}