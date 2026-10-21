import { useEffect, useState } from 'react';
import { getMedicines, addMedicine, updateMedicine, recordSale, getSales } from './api';
import MedicineGrid from './MedicineGrid';
import AddMedicineForm from './AddMedicineForm';
import SaleDialog from './SaleDialog';
import EditMedicineDialog from './EditMedicineDialog';
import SalesRecordsDialog from './SalesRecordsDialog';
import './App.css';

export default function App() {
  const [medicines, setMedicines] = useState([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saleTarget, setSaleTarget] = useState(null);
  const [editTarget, setEditTarget] = useState(null);
  const [showSales, setShowSales] = useState(false);
  const [sales, setSales] = useState([]);
  const [salesLoading, setSalesLoading] = useState(false);
  const [salesError, setSalesError] = useState('');

  async function loadMedicines(searchTerm) {
    try {
      setLoading(true);
      const data = await getMedicines(searchTerm);
      setMedicines(data);
      setError('');
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    const timeoutId = setTimeout(() => loadMedicines(search), 300);
    return () => clearTimeout(timeoutId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search]);

  async function handleAdd(medicine) {
    await addMedicine(medicine);
    await loadMedicines(search);
  }

  async function handleConfirmSale(medicineId, quantity) {
    await recordSale(medicineId, quantity);
    setSaleTarget(null);
    await loadMedicines(search);
  }

  async function handleConfirmEdit(medicineId, medicine) {
    await updateMedicine(medicineId, medicine);
    setEditTarget(null);
    await loadMedicines(search);
  }

  async function handleViewSales() {
    setShowSales(true);
    setSalesLoading(true);
    setSalesError('');
    try {
      const data = await getSales();
      setSales(data);
    } catch (err) {
      setSalesError(err.message);
    } finally {
      setSalesLoading(false);
    }
  }

  return (
    <div className="app">
      <h1>ABC Pharmacy - Medicine Inventory</h1>

      <AddMedicineForm onAdd={handleAdd} />

      <div className="list-section">
        <div className="list-header">
          <h2>Medicines</h2>
          <div className="list-header-actions">
            <input
              className="search-box"
              type="search"
              placeholder="Search by medicine name..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            <button type="button" onClick={handleViewSales}>Sale Records</button>
          </div>
        </div>

        <div className="legend">
          <span className="legend-item row-expiring">Expiring within 30 days</span>
          <span className="legend-item row-low-stock">Quantity below 10</span>
        </div>

        {error && <p className="form-error">{error}</p>}
        {loading ? (
          <p>Loading...</p>
        ) : (
          <MedicineGrid medicines={medicines} onSell={setSaleTarget} onEdit={setEditTarget} />
        )}
      </div>

      {saleTarget && (
        <SaleDialog
          medicine={saleTarget}
          onConfirm={handleConfirmSale}
          onCancel={() => setSaleTarget(null)}
        />
      )}

      {editTarget && (
        <EditMedicineDialog
          medicine={editTarget}
          onConfirm={handleConfirmEdit}
          onCancel={() => setEditTarget(null)}
        />
      )}

      {showSales && (
        <SalesRecordsDialog
          sales={sales}
          loading={salesLoading}
          error={salesError}
          onClose={() => setShowSales(false)}
        />
      )}
    </div>
  );
}

