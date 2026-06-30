USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_GetShipmentByAWB
(
    @AWBNumber NVARCHAR(20)
)
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
    WHERE AWBNumber = @AWBNumber;
END;
GO
EXEC usp_GetShipmentByAWB
    @AWBNumber = 'DEL2025001';