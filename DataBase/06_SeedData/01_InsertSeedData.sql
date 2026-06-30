USE DelhiveryDSIP;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Shipments)
BEGIN
    INSERT INTO dbo.Shipments
    (
        AWBNumber,
        SenderName,
        ReceiverName,
        Origin,
        Destination,
        WeightKg,
        Status,
        DeliveredAt
    )
    VALUES
    ('DEL2025001','Rahul Sharma','Amit Kumar','Hyderabad','Bangalore',2.50,'Booked',NULL),

    ('DEL2025002','Suresh Reddy','Priya Singh','Delhi','Mumbai',5.20,'InTransit',NULL),

    ('DEL2025003','John Peter','David Raj','Chennai','Pune',3.40,'Out for Delivery',NULL),

    ('DEL2025004','Anil Kumar','Ravi Teja','Hyderabad','Chennai',6.10,'Delivered',GETDATE()),

    ('DEL2025005','Sneha Rao','Kiran Das','Mumbai','Kolkata',1.80,'Delivered',GETDATE()),

    ('DEL2025006','Deepak Jain','Neha Gupta','Jaipur','Delhi',4.70,'RTO',NULL),

    ('DEL2025007','Meena Patel','Arjun Shah','Ahmedabad','Surat',2.90,'Booked',NULL),

    ('DEL2025008','Vijay Kumar','Lakshmi Devi','Vizag','Hyderabad',8.50,'In Transit',NULL);
END
GO
UPDATE Shipments
SET Status = 'InTransit'
WHERE Status = 'In Transit';

UPDATE Shipments
SET Status = 'OutForDelivery'
WHERE Status = 'Out for Delivery';SELECT * FROM Shipments;
EXEC usp_GetShipmentByAWB
    @AWBNumber = 'DEL2025004';
    EXEC usp_UpdateShipmentStatus
    @AWBNumber = 'DEL2025002',
    @NewStatus = 'Delivered';
SELECT
    AWBNumber,
    Status,
    DeliveredAt
FROM Shipments
WHERE AWBNumber='DEL2025002';

DELETE FROM Shipments;
DBCC CHECKIDENT ('Shipments', RESEED, 0);