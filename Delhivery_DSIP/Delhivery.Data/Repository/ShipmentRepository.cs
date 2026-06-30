using Delhivery.Data.DBHelper;
using Delhivery.Data.Interfaces;
using Delhivery.Domain.Entities;
using Delhivery.Domain.Enums;
using Delhivery.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Delhivery.Data.Repository
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly DbHelper _dbHelper;

        public ShipmentRepository()
        {
            _dbHelper = new DbHelper();
        }

        public List<Shipment> GetAll()
        {
            List<Shipment> shipments = new List<Shipment>();

            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand("usp_GetAllShipments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Shipment shipment = new Shipment
                            {
                                ShipmentId = Convert.ToInt32(reader["ShipmentId"]),
                                AWBNumber = reader["AWBNumber"].ToString(),
                                SenderName = reader["SenderName"].ToString(),
                                ReceiverName = reader["ReceiverName"].ToString(),
                                Origin = reader["Origin"].ToString(),
                                Destination = reader["Destination"].ToString(),
                                WeightKg = Convert.ToDouble(reader["WeightKg"]),

                                Status = (ShipmentStatus)Enum.Parse(
                                    typeof(ShipmentStatus),
                                    reader["Status"].ToString()
                                ),

                                BookedAt = Convert.ToDateTime(reader["BookedAt"])
                            };

                               if (reader["DeliveredAt"] != DBNull.Value)
                            {
                                shipment.DeliveredAt = Convert.ToDateTime(reader["DeliveredAt"]);
                            }

                            shipments.Add(shipment);
                        }
                    }
                }
            }

            return shipments;
        }

        public Shipment GetByAWB(string awb)
        {
            Shipment shipment = null;

            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand("usp_GetShipmentByAWB", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AWBNumber", awb);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shipment = new Shipment
                            {
                                ShipmentId = Convert.ToInt32(reader["ShipmentId"]),
                                AWBNumber = reader["AWBNumber"].ToString(),
                                SenderName = reader["SenderName"].ToString(),
                                ReceiverName = reader["ReceiverName"].ToString(),
                                Origin = reader["Origin"].ToString(),
                                Destination = reader["Destination"].ToString(),
                                WeightKg = Convert.ToDouble(reader["WeightKg"]),
                                Status = (ShipmentStatus)Enum.Parse(
                                            typeof(ShipmentStatus),
                                            reader["Status"].ToString(),
                                            true
                                ),
                                BookedAt = Convert.ToDateTime(reader["BookedAt"])
                            };

                            if (reader["DeliveredAt"] != DBNull.Value)
                            {
                                shipment.DeliveredAt = Convert.ToDateTime(reader["DeliveredAt"]);
                            }
                        }
                    }
                }
            }

            return shipment;
        }

        public void Book(Shipment shipment)
        {
            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                string query = @"INSERT INTO Shipments
                        (AWBNumber,
                         SenderName,
                         ReceiverName,
                         Origin,
                         Destination,
                         WeightKg,
                         Status,
                         BookedAt)
                        VALUES
                        (@AWBNumber,
                         @SenderName,
                         @ReceiverName,
                         @Origin,
                         @Destination,
                         @WeightKg,
                         @Status,
                         @BookedAt)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AWBNumber", shipment.AWBNumber);
                    command.Parameters.AddWithValue("@SenderName", shipment.SenderName);
                    command.Parameters.AddWithValue("@ReceiverName", shipment.ReceiverName);
                    command.Parameters.AddWithValue("@Origin", shipment.Origin);
                    command.Parameters.AddWithValue("@Destination", shipment.Destination);
                    command.Parameters.AddWithValue("@WeightKg", shipment.WeightKg);
                    command.Parameters.AddWithValue("@Status", shipment.Status.ToString());
                    command.Parameters.AddWithValue("@BookedAt", shipment.BookedAt);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateStatus(string awb, string status)
        {
            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                string query;

                if (status == "Delivered")
                {
                    query = @"UPDATE Shipments
                      SET Status = @Status,
                          DeliveredAt = GETDATE()
                      WHERE AWBNumber = @AWBNumber";
                }
                else
                {
                    query = @"UPDATE Shipments
                      SET Status = @Status
                      WHERE AWBNumber = @AWBNumber";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@AWBNumber", awb);

                    connection.Open();

                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        throw new ShipmentNotFoundException();
                    }
                }
            }
        }
        public void Cancel(int shipmentId)
        {
            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                string query = @"DELETE FROM Shipments
                         WHERE ShipmentId = @ShipmentId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ShipmentId", shipmentId);

                    connection.Open();

                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        throw new ShipmentNotFoundException();
                    }
                }
            }
        }
        public ShipmentStats GetShipmentStats()
        {
            ShipmentStats stats = new ShipmentStats();

            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand("usp_GetShipmentStats", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        stats.Booked = Convert.ToInt32(reader["Booked"]);
                        stats.InTransit = Convert.ToInt32(reader["InTransit"]);
                        stats.OutForDelivery = Convert.ToInt32(reader["OutForDelivery"]);
                        stats.Delivered = Convert.ToInt32(reader["Delivered"]);
                        stats.RTO = Convert.ToInt32(reader["RTO"]);
                    }
                }
            }

            return stats;
        }
    }
}