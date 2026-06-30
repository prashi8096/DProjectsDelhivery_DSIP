USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_UpdateShipmentStatus
(
    @AWBNumber NVARCHAR(20),
    @NewStatus NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Shipments
    SET
        Status = @NewStatus,
        DeliveredAt =
            CASE
                WHEN @NewStatus = 'Delivered'
                THEN GETDATE()
                ELSE DeliveredAt
            END
    WHERE AWBNumber = @AWBNumber;
END;
GO