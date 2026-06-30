USE DelhiveryDSIP;
GO

IF OBJECT_ID('dbo.Shipments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Shipments
    (
        ShipmentId INT IDENTITY(1,1) PRIMARY KEY,

        AWBNumber NVARCHAR(20) NOT NULL UNIQUE,

        SenderName NVARCHAR(100) NOT NULL,

        ReceiverName NVARCHAR(100) NOT NULL,

        Origin NVARCHAR(100) NOT NULL,

        Destination NVARCHAR(100) NOT NULL,

        WeightKg DECIMAL(8,2) NOT NULL,

        Status NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Shipments_Status
            DEFAULT ('Booked'),

        BookedAt DATETIME NOT NULL
            CONSTRAINT DF_Shipments_BookedAt
            DEFAULT(GETDATE()),

        DeliveredAt DATETIME NULL
    );
END
GO
SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Shipments';

sp_help Shipments;