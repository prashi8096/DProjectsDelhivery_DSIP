using Delhivery.Domain.Entities;
using System.Collections.Generic;

namespace Delhivery.Data.Interfaces
{
    public interface IShipmentRepository
    {
        List<Shipment> GetAll();

        Shipment GetByAWB(string awb);

        void Book(Shipment shipment);

        void UpdateStatus(string awb, string status);

        void Cancel(int shipmentId);
        ShipmentStats GetShipmentStats();
    }
}