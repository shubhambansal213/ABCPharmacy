# ABC Pharmacy - Medicine Inventory & Sales

A Single Page Application for ABC Pharmacy to track medicines and sales, built with:

- **Backend**: ASP.NET Core Web API on .NET 10 (data stored as JSON files on disk — no database)
- **Frontend**: React 18 (Vite)

## Features

- View all medicines in a grid (Full Name, Expiry Date, Quantity, Price, Brand — Notes excluded)
- Row highlighting:
  - Red background — expiry date is less than 30 days away (including already expired)
  - Yellow background — quantity in stock is less than 10
  - Both conditions — the row is split red/yellow so neither warning is hidden
- Search medicines by name (case-insensitive, partial match)
- Add a new medicine
- Edit an existing medicine
- Record a sale (reduces stock and logs the sale)
- View all sale records with totals

### Business rules (enforced by the API)

- Full name and expiry date are required
- Quantity and price cannot be negative; price has at most 2 decimal places
- A sale must be for at least 1 unit and cannot exceed the stock available
- Simultaneous sales of the same medicine are processed one at a time, so stock can never be oversold

## Project Structure

```
ABCPharmacy/
├── run.sh / run.ps1 / run.cmd    One-command start scripts (see "Running the app")
├── PharmacyApi/                  ASP.NET Core Web API
│   ├── Controllers/              MedicinesController, SalesController - HTTP endpoints
│   ├── Services/                 MedicineService - business rules (validation, stock checks)
│   ├── Repositories/             JsonMedicineRepository - reads/writes the JSON files
│   ├── Models/                   Medicine, SaleRecord, SaleRequest
│   ├── Exceptions/               NotFoundException (404), DomainValidationException (400)
│   ├── Middleware/               ExceptionHandlingMiddleware - turns exceptions into JSON error responses
│   └── Data/                     medicines.json, sales.json (created with sample data on first run)
└── pharmacy-client/              React (Vite) SPA
    └── src/
        ├── api.js                    Fetch calls to the API
        ├── App.jsx                   Main page - state, search, dialogs
        ├── MedicineGrid.jsx          Grid with color-coded rows
        ├── MedicineForm.jsx          Shared fields and validation for adding/editing
        ├── AddMedicineForm.jsx       Add form (uses MedicineForm)
        ├── EditMedicineDialog.jsx    Edit dialog (uses MedicineForm)
        ├── SaleDialog.jsx            Sell a medicine
        └── SalesRecordsDialog.jsx    List of recorded sales
```

### Backend design

The API is a single project split into a few simple layers:

- **Controllers** only map HTTP requests to service calls. They contain no business logic and no try/catch.
- **MedicineService** holds all business rules. When a rule is broken it throws `DomainValidationException` (→ 400) or `NotFoundException` (→ 404).
- **JsonMedicineRepository** only reads and writes the JSON files, behind the `IMedicineRepository` interface, so it could be replaced by a database without changing the service or controllers. It is registered as a singleton and uses a lock so concurrent requests can't corrupt the files.
- **ExceptionHandlingMiddleware** catches every exception in one place and returns a consistent JSON error: `{ "status": 400, "message": "..." }`. Unexpected errors return a generic 500 without exposing internal details.

## Running the app

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org) 22 LTS or newer (20.19+ also works)

### Quick start (one command)

From the repository root:

| OS | Command |
|----|---------|
| Windows | Double-click `run.cmd` (or run `powershell -ExecutionPolicy Bypass -File run.ps1`) |
| macOS / Linux | `./run.sh` (or `bash run.sh`) |

The script checks that .NET and Node.js are installed, starts the API, installs the client packages on first run, starts the React app and opens `http://localhost:5173` in your browser. Press **Ctrl+C** to stop both.

API documentation (Swagger UI) is available at `http://localhost:5270/swagger` while the app is running.

### Manual start

#### 1. Start the API

```powershell
cd PharmacyApi
dotnet run
```

The API starts on `http://localhost:5270`. On first run it seeds `Data/medicines.json` with sample medicines.

#### 2. Start the React app

In a separate terminal:

```powershell
cd pharmacy-client
npm install
npm run dev
```

The app starts on `http://localhost:5173` and proxies `/api/*` calls to the API (configured in `vite.config.js`).

Open `http://localhost:5173` in your browser.

## API Endpoints

| Method | Route                      | Description                               |
|--------|----------------------------|-------------------------------------------|
| GET    | `/api/medicines`           | List all medicines (optional `?search=`)  |
| GET    | `/api/medicines/{id}`      | Get a single medicine                     |
| POST   | `/api/medicines`           | Add a new medicine                        |
| PUT    | `/api/medicines/{id}`      | Update an existing medicine               |
| POST   | `/api/medicines/{id}/sale` | Record a sale (`{ "quantity": n }`)       |
| GET    | `/api/sales`               | List all recorded sales                   |

Errors are returned as `{ "status": <code>, "message": "<reason>" }` with status 400 (invalid request), 404 (medicine not found) or 500 (unexpected error).

## Data Storage

Medicines and sales are persisted in plain JSON files under `PharmacyApi/Data/` (`medicines.json`, `sales.json`) — no database is used, per the assessment requirements. If the files don't exist, they are created on startup with sample medicines.
