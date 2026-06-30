USE DelhiveryDSIP;
GO

CREATE OR ALTER PROCEDURE usp_GetShipmentStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        SUM(CASE WHEN Status = 'Booked' THEN 1 ELSE 0 END) AS Booked,
        SUM(CASE WHEN Status = 'InTransit' THEN 1 ELSE 0 END) AS InTransit,
        SUM(CASE WHEN Status = 'OutForDelivery' THEN 1 ELSE 0 END) AS OutForDelivery,
        SUM(CASE WHEN Status = 'Delivered' THEN 1 ELSE 0 END) AS Delivered,
        SUM(CASE WHEN Status = 'RTO' THEN 1 ELSE 0 END) AS RTO
    FROM Shipments;
END
GO
SELECT DISTINCT Status
FROM Shipments;