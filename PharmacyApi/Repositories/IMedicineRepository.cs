using PharmacyApi.Models;

namespace PharmacyApi.Repositories;

/// <summary>
/// Data access abstraction for medicines and sale records.
/// Implementations only handle persistence - no business/validation rules belong here.
/// </summary>
public interface IMedicineRepository
{
    List<Medicine> GetAllMedicines();

    Medicine? GetMedicineById(int id);

    Medicine AddMedicine(Medicine medicine);

    /// <summary>
    /// Persists changes to an existing medicine. Returns null if no medicine with that id exists.
    /// </summary>
    Medicine? UpdateMedicine(Medicine medicine);

    List<SaleRecord> GetAllSales();

    /// <summary>
    /// Atomically persists the updated medicine stock level together with the new sale record.
    /// </summary>
    SaleRecord SaveSale(Medicine updatedMedicine, SaleRecord sale);
}
