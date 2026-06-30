USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_UpdateShipmentStatus
(
    @AWBNumber NVARCHAR(20),
    @Status NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Shipments
    SET
        Status = @Status,
        DeliveredAt =
        CASE
            WHEN @Status = 'Delivered'
            THEN GETDATE()
            ELSE DeliveredAt
        END
    WHERE AWBNumber = @AWBNumber;
END
GO
select * from Shipments