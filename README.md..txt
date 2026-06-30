# Delhivery Shipment Intelligence Platform (DSIP)

## Project Overview

The Delhivery Shipment Intelligence Platform (DSIP) is a Shipment Management System developed as part of the Delhivery Assessment. The application demonstrates the implementation of C#, ADO.NET, SQL Server, ASP.NET Core Web API, HTML, CSS, JavaScript, jQuery, and Python in a layered architecture.

The project allows users to book shipments, update shipment status, track shipments, cancel shipments, view shipment statistics, and generate end-of-day analytical reports.

---

## Technologies Used

- C#
- .NET 8
- ASP.NET Core Web API
- ADO.NET
- SQL Server
- HTML5
- CSS3
- JavaScript
- jQuery
- Python 3
- Requests Library

---

## Project Structure

```
Delhivery_DSIP

│
├── Database
│
├── Delhivery.Console
│
├── Delhivery.API
│
├── Delhivery.Data
│
├── Delhivery.Domain
│
├── UI
│
├── Python
│     ├── report.py
│     └── delhivery_report_YYYYMMDD.csv
│
├── GenAI
│     ├── COPILOT_LOG.md
│     ├── PROMPT_LOG.md
│     └── REFLECTION.md
│
└── README.md
```

---

## Features

### Console Application

- Book Shipment
- View All Shipments
- Search Shipment by AWB
- Update Shipment Status
- Cancel Shipment
- View Shipment Statistics

---

### Web API

Available Endpoints

| Method | Endpoint |
|---------|----------|
| GET | /api/Shipment |
| GET | /api/Shipment/{awb} |
| POST | /api/Shipment |
| PUT | /api/Shipment/{awb} |
| DELETE | /api/Shipment/{id} |
| GET | /api/Shipment/stats |

---

### Web UI

- Dashboard
- Book Shipment
- Track Shipment
- Update Shipment Status
- Cancel Shipment
- Status Filter
- Statistics Cards
- Responsive Layout

---

### Python Analytics

The report.py script performs the following:

- Fetches shipment data from the Web API
- Calculates shipment statistics
- Finds average shipment weight
- Finds the heaviest shipment
- Displays End-of-Day Report
- Exports shipment details to CSV using the --export option

---

## Database Setup

1. Open SQL Server Management Studio.

2. Create the database.

```
DelhiveryDB
```

3. Execute all SQL scripts located inside the Database folder.

4. Verify that the Shipments table has been created successfully.

---

## Update Connection String

Open

```
appsettings.json
```

Update

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=DelhiveryDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Running the Console Application

1. Open the solution in Visual Studio.

2. Set

```
Delhivery.Console
```

as Startup Project.

3. Press

```
F5
```

---

## Running the Web API

1. Set

```
Delhivery.API
```

as Startup Project.

2. Run the project.

3. Swagger opens automatically.

Example

```
https://localhost:7113/swagger
```

---

## Running the UI

1. Open the UI folder.

2. Launch

```
index.html
```

using Live Server.

OR

```
http://127.0.0.1:3000/
```

3. Ensure the API is running before opening the UI.

---

## Running the Python Report

Navigate to the Python folder.

Generate report

```
python report.py
```

Generate report and export CSV

```
python report.py --export
```

---

## Business Rules

- Every shipment must have a unique AWB Number.
- Shipment weight must be greater than zero.
- Shipment status can move only in the forward direction.
- Delivered shipments cannot be updated.
- Cancelled shipments are removed from the system.
- Duplicate AWB numbers are not allowed.

---

## Error Handling

The application handles:

- Invalid Shipment
- Shipment Not Found
- Duplicate AWB
- API Connection Failure
- Database Exceptions
- Python API Offline Handling

---

## AI Usage

GitHub Copilot and ChatGPT were used for:

- Code suggestions
- Debugging
- Documentation
- Python report generation
- UI improvements

All generated code was manually reviewed, tested, and modified where necessary.

---

## Author

Prashanth

Delhivery DSIP Assessment Project