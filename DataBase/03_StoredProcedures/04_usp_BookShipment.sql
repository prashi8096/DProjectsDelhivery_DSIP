USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_BookShipment
(
    @AWBNumber NVARCHAR(20),
    @SenderName NVARCHAR(100),
    @ReceiverName NVARCHAR(100),
    @Origin NVARCHAR(100),
    @Destination NVARCHAR(100),
    @WeightKg DECIMAL(8,2),
    @Status NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Shipments
    (
        AWBNumber,
        SenderName,
        ReceiverName,
        Origin,
        Destination,
        WeightKg,
        Status
    )
    VALUES
    (
        @AWBNumber,
        @SenderName,
        @ReceiverName,
        @Origin,
        @Destination,
        @WeightKg,
        @Status
    );
END
GO