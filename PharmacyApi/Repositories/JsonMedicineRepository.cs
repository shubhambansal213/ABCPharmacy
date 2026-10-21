using System.Text.Json;
using PharmacyApi.Models;

namespace PharmacyApi.Repositories;

/// <summary>
/// JSON-file-backed implementation of <see cref="IMedicineRepository"/>.
/// Not a database - reads/writes the whole file on every operation, guarded by a lock.
/// Contains only persistence logic; business rules live in the service layer.
/// </summary>
public class JsonMedicineRepository : IMedicineRepository
{
    private readonly string _medicinesFilePath;
    private readonly string _salesFilePath;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonMedicineRepository(IWebHostEnvironment env)
    {
        var dataFolder = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataFolder);
        _medicinesFilePath = Path.Combine(dataFolder, "medicines.json");
        _salesFilePath = Path.Combine(dataFolder, "sales.json");

        if (!File.Exists(_medicinesFilePath))
        {
            SeedInitialMedicines();
        }

        if (!File.Exists(_salesFilePath))
        {
            File.WriteAllText(_salesFilePath, "[]");
        }
    }

    private void SeedInitialMedicines()
    {
        var seed = new List<Medicine>
        {
            new() { Id = 1, FullName = "Paracetamol 500mg", Notes = "For fever and pain relief", ExpiryDate = DateTime.Today.AddDays(5), Quantity = 50, Price = 25.50m, Brand = "Cipla" },
            new() { Id = 2, FullName = "Amoxicillin 250mg", Notes = "Antibiotic", ExpiryDate = DateTime.Today.AddDays(200), Quantity = 8, Price = 60.00m, Brand = "Sun Pharma" },
            new() { Id = 3, FullName = "Cetirizine 10mg", Notes = "Antihistamine for allergies", ExpiryDate = DateTime.Today.AddDays(15), Quantity = 5, Price = 12.75m, Brand = "Dr. Reddy's" },
            new() { Id = 4, FullName = "Vitamin C 500mg", Notes = "Immunity booster", ExpiryDate = DateTime.Today.AddDays(365), Quantity = 100, Price = 45.00m, Brand = "HealthVit" },
        };
        File.WriteAllText(_medicinesFilePath, JsonSerializer.Serialize(seed, _jsonOptions));
    }

    public List<Medicine> GetAllMedicines()
    {
        lock (_lock)
        {
            return ReadMedicines();
        }
    }

    public Medicine? GetMedicineById(int id)
    {
        lock (_lock)
        {
            return ReadMedicines().FirstOrDefault(m => m.Id == id);
        }
    }

    public Medicine AddMedicine(Medicine medicine)
    {
        lock (_lock)
        {
            var medicines = ReadMedicines();
            medicine.Id = medicines.Count == 0 ? 1 : medicines.Max(m => m.Id) + 1;
            medicines.Add(medicine);
            WriteMedicines(medicines);
            return medicine;
        }
    }

    public Medicine? UpdateMedicine(Medicine medicine)
    {
        lock (_lock)
        {
            var medicines = ReadMedicines();
            var index = medicines.FindIndex(m => m.Id == medicine.Id);
            if (index < 0)
            {
                return null;
            }

            medicines[index] = medicine;
            WriteMedicines(medicines);
            return medicine;
        }
    }

    public List<SaleRecord> GetAllSales()
    {
        lock (_lock)
        {
            return ReadSales();
        }
    }

    public SaleRecord SaveSale(Medicine updatedMedicine, SaleRecord sale)
    {
        lock (_lock)
        {
            var medicines = ReadMedicines();
            var index = medicines.FindIndex(m => m.Id == updatedMedicine.Id);
            if (index >= 0)
            {
                medicines[index] = updatedMedicine;
                WriteMedicines(medicines);
            }

            var sales = ReadSales();
            sale.Id = sales.Count == 0 ? 1 : sales.Max(s => s.Id) + 1;
            sales.Add(sale);
            WriteSales(sales);

            return sale;
        }
    }

    private List<Medicine> ReadMedicines()
    {
        var json = File.ReadAllText(_medicinesFilePath);
        return JsonSerializer.Deserialize<List<Medicine>>(json, _jsonOptions) ?? [];
    }

    private void WriteMedicines(List<Medicine> medicines)
    {
        File.WriteAllText(_medicinesFilePath, JsonSerializer.Serialize(medicines, _jsonOptions));
    }

    private List<SaleRecord> ReadSales()
    {
        var json = File.ReadAllText(_salesFilePath);
        return JsonSerializer.Deserialize<List<SaleRecord>>(json, _jsonOptions) ?? [];
    }

    private void WriteSales(List<SaleRecord> sales)
    {
        File.WriteAllText(_salesFilePath, JsonSerializer.Serialize(sales, _jsonOptions));
    }
}
