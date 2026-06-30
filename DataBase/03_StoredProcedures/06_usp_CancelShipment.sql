USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_CancelShipment
(
    @AWBNumber NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Shipments
    WHERE AWBNumber = @AWBNumber;
END
GO



EXEC usp_CancelShipment 'AWB2001'
SELECT *
FROM Shipments
WHERE AWBNumber = 'AWB2001';
