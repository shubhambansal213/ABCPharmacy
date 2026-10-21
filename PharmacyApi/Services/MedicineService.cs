using PharmacyApi.Exceptions;
using PharmacyApi.Models;
using PharmacyApi.Repositories;

namespace PharmacyApi.Services;

public class MedicineService : IMedicineService
{
    // Static because the service is created per request; all sales must share one lock.
    private static readonly object SaleLock = new();

    private readonly IMedicineRepository _repository;

    public MedicineService(IMedicineRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Medicine> GetAll(string? search)
    {
        var medicines = _repository.GetAllMedicines();

        if (!string.IsNullOrWhiteSpace(search))
        {
            medicines = medicines
                .Where(m => m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return medicines;
    }

    public Medicine GetById(int id)
    {
        return _repository.GetMedicineById(id)
            ?? throw new NotFoundException($"Medicine with id {id} was not found.");
    }

    public Medicine Add(Medicine medicine)
    {
        Validate(medicine);

        return _repository.AddMedicine(medicine);
    }

    public Medicine Update(int id, Medicine medicine)
    {
        Validate(medicine);

        medicine.Id = id;

        return _repository.UpdateMedicine(medicine)
            ?? throw new NotFoundException($"Medicine with id {id} was not found.");
    }

    public SaleRecord RecordSale(int medicineId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainValidationException("Quantity must be greater than zero.");
        }

        // Read stock, check it and save the sale as one step, so two simultaneous
        // sales can't both pass the stock check and oversell.
        lock (SaleLock)
        {
            var medicine = _repository.GetMedicineById(medicineId)
                ?? throw new NotFoundException($"Medicine with id {medicineId} was not found.");

            if (medicine.Quantity < quantity)
            {
                throw new DomainValidationException($"Only {medicine.Quantity} unit(s) of '{medicine.FullName}' are in stock.");
            }

            medicine.Quantity -= quantity;

            var sale = new SaleRecord
            {
                MedicineId = medicine.Id,
                MedicineName = medicine.FullName,
                QuantitySold = quantity,
                UnitPrice = medicine.Price,
                SaleDate = DateTime.Now,
            };

            return _repository.SaveSale(medicine, sale);
        }
    }

    public IEnumerable<SaleRecord> GetAllSales()
    {
        return _repository.GetAllSales();
    }

    /// <summary>
    /// Server-side rules for a medicine's fields. The client form enforces the same rules,
    /// but the API must not rely on that since it can be called directly.
    /// </summary>
    private static void Validate(Medicine medicine)
    {
        if (string.IsNullOrWhiteSpace(medicine.FullName))
        {
            throw new DomainValidationException("Full name is required.");
        }

        if (medicine.ExpiryDate == default)
        {
            throw new DomainValidationException("Expiry date is required.");
        }

        if (medicine.Quantity < 0)
        {
            throw new DomainValidationException("Quantity cannot be negative.");
        }

        if (medicine.Price < 0)
        {
            throw new DomainValidationException("Price cannot be negative.");
        }

        if (decimal.Round(medicine.Price, 2) != medicine.Price)
        {
            throw new DomainValidationException("Price can have at most 2 decimal places.");
        }
    }
}
