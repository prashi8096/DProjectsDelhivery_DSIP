using System;

namespace Delhivery.Domain.Exceptions
{
    public class InvalidShipmentException : Exception
    {
        public InvalidShipmentException()
            : base("Invalid shipment details.")
        {
        }

        public InvalidShipmentException(string message)
            : base(message)
        {
        }
    }
}