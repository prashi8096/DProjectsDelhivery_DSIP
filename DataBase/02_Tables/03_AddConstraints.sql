USE DelhiveryDSIP;
GO

-- Weight must be greater than zero
IF NOT EXISTS
(
    SELECT *
    FROM sys.check_constraints
    WHERE name = 'CK_Shipments_WeightKg'
)
BEGIN
    ALTER TABLE dbo.Shipments
    ADD CONSTRAINT CK_Shipments_WeightKg
    CHECK (WeightKg > 0);
END
GO

-- Status validation
IF OBJECT_ID('dbo.CK_Shipments_Status', 'C') IS NOT NULL
    -- 2. Add the updated constraint with your changes
ALTER TABLE dbo.Shipments
ADD CONSTRAINT CK_Shipments_Status
CHECK (
    Status IN (
        'Booked',
        'InTransit',
        'OutForDelivery', -- Fixed: Added spaces back
        'Delivered',
        'RTO'
    )
);
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
    'TEST001',
    'Rahul',
    'Amit',
    'Hyderabad',
    'Bangalore',
    -5,
    'Booked'
);

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
    'TEST002',
    'Rahul',
    'Amit',
    'Hyderabad',
    'Bangalore',
    2.5,
    'Processing'
);