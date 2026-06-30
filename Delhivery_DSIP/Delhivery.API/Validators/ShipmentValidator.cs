using Delhivery.Domain.Entities;

namespace Delhivery.API.Validators
{
    public static class ShipmentValidator
    {
        public static string Validate(Shipment shipment)
        {
            if (string.IsNullOrWhiteSpace(shipment.AWBNumber))
                return "AWB Number is required.";

            if (string.IsNullOrWhiteSpace(shipment.SenderName))
                return "Sender Name is required.";

            if (string.IsNullOrWhiteSpace(shipment.ReceiverName))
                return "Receiver Name is required.";

            if (shipment.WeightKg <= 0)
                return "Weight must be greater than 0.";

            return string.Empty;
        }
    }
}