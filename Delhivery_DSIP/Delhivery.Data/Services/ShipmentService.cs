using Delhivery.Data.Interfaces;
using Delhivery.Data.Repository;
using Delhivery.Domain.Entities;
using Delhivery.Domain.Enums;
using Delhivery.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            // Trim all text fields up front so stored data never has
            // leading/trailing spaces, and validation checks see clean values
            shipment.AWBNumber = shipment.AWBNumber?.Trim();
            shipment.SenderName = shipment.SenderName?.Trim();
            shipment.ReceiverName = shipment.ReceiverName?.Trim();
            shipment.Origin = shipment.Origin?.Trim();
            shipment.Destination = shipment.Destination?.Trim();

            if (string.IsNullOrWhiteSpace(shipment.AWBNumber))
                throw new InvalidShipmentException("AWB Number is required.");

            // AWB Number - 5 to 20 letters/numbers only, no spaces or symbols
            if (!Regex.IsMatch(shipment.AWBNumber, "^[A-Za-z0-9]{5,20}$"))
                throw new InvalidShipmentException(
                    "AWB Number must be 5-20 letters/numbers only (no spaces or symbols).");

            if (string.IsNullOrWhiteSpace(shipment.SenderName))
                throw new InvalidShipmentException("Sender Name is required.");

            // Sender Name - letters and spaces only, 2 to 50 characters
            if (!Regex.IsMatch(shipment.SenderName, "^[A-Za-z ]{2,50}$"))
                throw new InvalidShipmentException(
                    "Sender Name must contain only letters and spaces (2-50 characters).");

            if (string.IsNullOrWhiteSpace(shipment.ReceiverName))
                throw new InvalidShipmentException("Receiver Name is required.");

            // Receiver Name - letters and spaces only, 2 to 50 characters
            if (!Regex.IsMatch(shipment.ReceiverName, "^[A-Za-z ]{2,50}$"))
                throw new InvalidShipmentException(
                    "Receiver Name must contain only letters and spaces (2-50 characters).");

            if (string.IsNullOrWhiteSpace(shipment.Origin))
                throw new InvalidShipmentException("Origin is required.");

            // Origin - letters, numbers, spaces, commas, dots and hyphens only
            // (allows city names, pincodes, and combos like "Sector 5, Pune")
            if (!Regex.IsMatch(shipment.Origin, "^[A-Za-z0-9 ,.-]{2,50}$"))
                throw new InvalidShipmentException(
                    "Origin must be a valid place name (2-50 characters).");

            if (string.IsNullOrWhiteSpace(shipment.Destination))
                throw new InvalidShipmentException("Destination is required.");

            // Destination - same pattern as Origin
            if (!Regex.IsMatch(shipment.Destination, "^[A-Za-z0-9 ,.-]{2,50}$"))
                throw new InvalidShipmentException(
                    "Destination must be a valid place name (2-50 characters).");

            // Origin and Destination cannot be the same
            if (string.Equals(shipment.Origin, shipment.Destination, StringComparison.OrdinalIgnoreCase))
                throw new InvalidShipmentException("Origin and Destination cannot be the same.");

            if (shipment.WeightKg <= 0)
                throw new InvalidShipmentException("Weight must be greater than zero.");

            // Weight - cannot exceed 1000 Kg
            if (shipment.WeightKg > 1000)
                throw new InvalidShipmentException("Weight cannot be more than 1000 Kg.");

            // Weight - at most 2 decimal places (avoids DB truncation/overflow errors)
            if (Math.Round(shipment.WeightKg, 2) != shipment.WeightKg)
                throw new InvalidShipmentException("Weight can have at most 2 decimal places.");
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
            // Guard against null/empty AWB before hitting the repository,
            // so a bad input fails with a clean message instead of a raw exception
            if (string.IsNullOrWhiteSpace(awb))
                throw new InvalidShipmentException("AWB Number is required.");

            return _repository.GetByAWB(awb.Trim());
        }

        public void UpdateStatus(string awb, ShipmentStatus newStatus)
        {
            // Guard against null/empty AWB before hitting the repository
            if (string.IsNullOrWhiteSpace(awb))
                throw new InvalidShipmentException("AWB Number is required.");

            if (!Enum.IsDefined(typeof(ShipmentStatus), newStatus))
            {
                throw new InvalidShipmentException(
                    "Invalid Status. Valid values are: Booked, InTransit, OutForDelivery, Delivered, RTO.");
            }

            // Get current shipment
            Shipment shipment = _repository.GetByAWB(awb.Trim());

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
            // Guard against an invalid ID before hitting the repository
            if (shipmentId <= 0)
                throw new InvalidShipmentException("A valid Shipment ID is required.");

            _repository.Cancel(shipmentId);
        }
        public ShipmentStats GetShipmentStats()
        {
            return _repository.GetShipmentStats();
        }
    }
}