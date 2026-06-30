USE DelhiveryDSIP;
GO

IF NOT EXISTS
(
    SELECT *
    FROM sys.indexes
    WHERE name = 'IX_Shipments_Status'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Shipments_Status
    ON dbo.Shipments(Status);
END
GO