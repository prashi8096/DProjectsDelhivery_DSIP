USE DelhiveryDSIP;
GO

CREATE OR ALTER VIEW vw_ShipmentDashboard
AS
SELECT
    AWBNumber,
    SenderName,
    ReceiverName,
    Origin,
    Destination,
    Status,
    BookedAt
FROM dbo.Shipments;
GO
SELECT * FROM vw_ShipmentDashboard;