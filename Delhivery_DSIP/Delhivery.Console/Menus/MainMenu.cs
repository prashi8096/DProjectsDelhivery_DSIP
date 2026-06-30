using Delhivery.ConsoleApp.Helpers;
using Delhivery.Data.Services;

//using Delhivery.ConsoleAPP.Services;
using Delhivery.Domain.Entities;
using Delhivery.Domain.Enums;
using System;

namespace Delhivery.ConsoleApp.Menus
{
    public class MainMenu
    {
        private readonly ShipmentService _shipmentService;

        public MainMenu(ShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public void Start()
        {

            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine("========================================");
                Console.WriteLine(" DELHIVERY SHIPMENT MANAGEMENT SYSTEM");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Book Shipment");
                Console.WriteLine("2. List Shipments");
                Console.WriteLine("3. Update Shipment Status");
                Console.WriteLine("4. Cancel Shipment");
                Console.WriteLine("0. Exit");
                Console.WriteLine("========================================");

                Console.Write("Enter your choice : ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        try
                        {
                            DisplayHelper.ShowHeader("BOOK SHIPMENT");

                            Shipment shipment = new Shipment();

                            shipment.AWBNumber = InputHelper.ReadString("Enter AWB Number: ");

                            shipment.SenderName = InputHelper.ReadString("Enter Sender Name: ");

                           
                            shipment.ReceiverName = InputHelper.ReadString("Enter Receiver Name: ");

                            shipment.Origin = InputHelper.ReadString("Enter Orgin: ");

                            shipment.Destination = InputHelper.ReadString("Enter Destination: ");

                            shipment.WeightKg = InputHelper.ReadDouble("Enter Weight (Kg): ");

                            shipment.Status = ShipmentStatus.Booked;

                            _shipmentService.BookShipment(shipment);

                            DisplayHelper.ShowSuccess("\nShipment booked successfully.");
                        }
                        catch (Exception ex)
                        {
                            DisplayHelper.ShowError($"\nError : {ex.Message}");
                        }

                        DisplayHelper.Pause();
                        break;
                    case "2":

                        Console.Clear();
                        Console.WriteLine("========== SHIPMENT LIST ==========\n");

                        var shipments = _shipmentService.ListShipments();

                        if (shipments.Count == 0)
                        {
                            Console.WriteLine("No shipments available.");
                        }
                        else
                        {
                            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine(
     $"{"AWB",-12} {"Sender",-15} {"Receiver",-15} {"Origin",-12} {"Destination",-15} {"Weight",-8} {"Status",-15} {"Booked",-22} {"Delivered",-22}");
                            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------");

                            foreach (var shipment in shipments)
                            {
                                string deliveredAt = "-";

                                if (shipment.DeliveredAt.HasValue)
                                {
                                    deliveredAt = shipment.DeliveredAt.Value.ToString("dd-MM-yyyy HH:mm");
                                }
                                Console.WriteLine(
    $"{shipment.AWBNumber,-12}" +
    $"{shipment.SenderName,-15}" +
    $"{shipment.ReceiverName,-15}" +
    $"{shipment.Origin,-12}" +
    $"{shipment.Destination,-15}" +
    $"{shipment.WeightKg,-8}" +
    $"{shipment.Status,-18}" +
    $"{shipment.BookedAt.ToString("dd-MM-yyyy HH:mm"),-20}" +
    $"{deliveredAt,-20}");
                            }

                            Console.WriteLine("-----------------------------------------------------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"\nTotal Shipments : {shipments.Count}");
                            Console.ResetColor();
                        }

                        DisplayHelper.Pause();
                        break;
                    case "3":

                        try
                        {
                            DisplayHelper.ShowHeader("UPDATE SHIPMENT STATUS");

                            Console.Write("Enter AWB Number: ");
                            string awb = Console.ReadLine();

                            Console.WriteLine();
                            Console.WriteLine("Select New Status");
                            Console.WriteLine("1. Booked");
                            Console.WriteLine("2. InTransit");
                            Console.WriteLine("3. OutForDelivery");
                            Console.WriteLine("4. Delivered");
                            Console.WriteLine("5. RTO");

                          

                            int statusChoice = InputHelper.ReadInteger("Choice: ");

                            ShipmentStatus? status = null;

                            switch (statusChoice)
                            {
                                case 1:
                                    status = ShipmentStatus.Booked;
                                    break;

                                case 2:
                                    status = ShipmentStatus.InTransit;
                                    break;

                                case 3:
                                    status = ShipmentStatus.OutForDelivery;
                                    break;

                                case 4:
                                    status = ShipmentStatus.Delivered;
                                    break;

                                case 5:
                                    status = ShipmentStatus.RTO;
                                    break;

                                default:
                                    Console.WriteLine("Invalid Status.");
                                    Console.ReadKey();
                                    break;
                            }

                            if (status == null)
                            {
                                break;
                            }

                            _shipmentService.UpdateStatus(awb, status.Value);

                            DisplayHelper.ShowSuccess("\nShipment updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            DisplayHelper.ShowError($"\nError : {ex.Message}");
                        }

                        DisplayHelper.Pause();
                        break;
                    case "4":

                        try
                        {
                            DisplayHelper.ShowHeader("CANCEL SHIPMENT");

                       
                            int sId = InputHelper.ReadInteger("Enter Shipment Number: ");


                            _shipmentService.CancelShipment(sId);

                            DisplayHelper.ShowSuccess("\nShipment cancled successfully.");
                        }
                        catch (Exception ex)
                        {
                            DisplayHelper.ShowError($"\nError : {ex.Message}");
                        }

                        DisplayHelper.Pause();
                        break;

                    case "0":

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nThank you for using Delhivery Shipment Management System.");
                        Console.ResetColor();

                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid Choice.");
                        Console.ReadKey();
                        break;
                }
            }

        }
    }
}