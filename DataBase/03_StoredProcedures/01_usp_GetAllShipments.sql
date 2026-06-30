USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_GetAllShipments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ShipmentId,
        AWBNumber,
        SenderName,
        ReceiverName,
        Origin,
        Destination,
        WeightKg,
        Status,
        BookedAt,
        DeliveredAt
    FROM Shipments
    ORDER BY BookedAt DESC;
END;
GO
EXEC usp_GetAllShipments;