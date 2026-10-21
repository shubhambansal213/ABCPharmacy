using PharmacyApi.Models;

namespace PharmacyApi.Services;

/// <summary>
/// Business/use-case operations for medicines and sales.
/// Validates input and enforces domain rules before delegating persistence to the repository.
/// </summary>
public interface IMedicineService
{
    IEnumerable<Medicine> GetAll(string? search);

    Medicine GetById(int id);

    Medicine Add(Medicine medicine);

    Medicine Update(int id, Medicine medicine);

    SaleRecord RecordSale(int medicineId, int quantity);

    IEnumerable<SaleRecord> GetAllSales();
}
