using System;

namespace Delhivery.Domain.Exceptions
{
    public class ShipmentNotFoundException : Exception
    {
        public ShipmentNotFoundException()
            : base("Shipment not found.")
        {
        }

        public ShipmentNotFoundException(string message)
            : base(message)
        {
        }
    }
}