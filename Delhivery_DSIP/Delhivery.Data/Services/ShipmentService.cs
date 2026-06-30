using Delhivery.Data.Interfaces;
using Delhivery.Data.Repository;
using Delhivery.Domain.Entities;
using Delhivery.Domain.Enums;
using Delhivery.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace Delhivery.Data.Services
{
    public class ShipmentService
    {
        private readonly IShipmentRepository _repository;

  

        public ShipmentService(IShipmentRepository repository)
        {
            _repository = repository;
        }

        private void ValidateShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new InvalidShipmentException("Shipment cannot be null.");

            if (string.IsNullOrWhiteSpace(shipment.AWBNumber))  
                throw new InvalidShipmentException("AWB Number is required.");

            if (string.IsNullOrWhiteSpace(shipment.SenderName))
                throw new InvalidShipmentException("Sender Name is required.");

            if (string.IsNullOrWhiteSpace(shipment.ReceiverName))
                throw new InvalidShipmentException("Receiver Name is required.");

            if (string.IsNullOrWhiteSpace(shipment.Origin))
                throw new InvalidShipmentException("Origin is required.");

            if (string.IsNullOrWhiteSpace(shipment.Destination))
                throw new InvalidShipmentException("Destination is required.");

            if (shipment.WeightKg <= 0)
                throw new InvalidShipmentException("Weight must be greater than zero.");
        }

        public void BookShipment(Shipment shipment)
        {
            ValidateShipment(shipment);
            shipment.BookedAt = DateTime.Now;
            shipment.Status = ShipmentStatus.Booked;

            _repository.Book(shipment);
        }

        public List<Shipment> ListShipments()
        {
            return _repository.GetAll();
        }

        public Shipment GetShipmentByAWB(string awb)
        {
            return _repository.GetByAWB(awb);
        }

        public void UpdateStatus(string awb, ShipmentStatus newStatus)
        {
            if (!Enum.IsDefined(typeof(ShipmentStatus), newStatus))
            {
                throw new InvalidShipmentException(
                    "Invalid Status. Valid values are: Booked, InTransit, OutForDelivery, Delivered, RTO.");
            }

            // Get current shipment
            Shipment shipment = _repository.GetByAWB(awb);

            ShipmentStatus currentStatus = shipment.Status;

            bool isValid = false;

            switch (currentStatus)
            {
                case ShipmentStatus.Booked:
                    isValid = (newStatus == ShipmentStatus.InTransit ||
                               newStatus == ShipmentStatus.RTO);
                    break;

                case ShipmentStatus.InTransit:
                    isValid = (newStatus == ShipmentStatus.OutForDelivery ||
                               newStatus == ShipmentStatus.RTO);
                    break;

                case ShipmentStatus.OutForDelivery:
                    isValid = (newStatus == ShipmentStatus.Delivered ||
                               newStatus == ShipmentStatus.RTO);
                    break;

                case ShipmentStatus.Delivered:
                case ShipmentStatus.RTO:
                    isValid = false;
                    break;
            }

            if (!isValid)
            {
                throw new InvalidShipmentException(
                    $"Status cannot be changed from {currentStatus} to {newStatus}.");
            }

            _repository.UpdateStatus(awb, newStatus.ToString());
        }

        public void CancelShipment(int shipmentId)
        {
            _repository.Cancel(shipmentId);
        }
        public ShipmentStats GetShipmentStats()
        {
            return _repository.GetShipmentStats();
        }
    }
}