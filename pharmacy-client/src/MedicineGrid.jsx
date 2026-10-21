const EXPIRY_WARNING_DAYS = 30;
const LOW_STOCK_THRESHOLD = 10;

function daysUntil(dateString) {
  const diffMs = new Date(dateString) - new Date();
  return Math.ceil(diffMs / (1000 * 60 * 60 * 24));
}

function getRowClasses(medicine) {
  const expiringSoon = daysUntil(medicine.expiryDate) < EXPIRY_WARNING_DAYS;
  const lowStock = medicine.quantity < LOW_STOCK_THRESHOLD;

  const classes = [];
  if (expiringSoon) classes.push('row-expiring');
  if (lowStock) classes.push('row-low-stock');
  return classes.join(' ');
}

export default function MedicineGrid({ medicines, onSell, onEdit }) {
  if (medicines.length === 0) {
    return <p className="empty-message">No medicines found.</p>;
  }

  return (
    <table className="medicine-grid">
      <thead>
        <tr>
          <th>Full Name</th>
          <th>Expiry Date</th>
          <th>Quantity</th>
          <th>Price</th>
          <th>Brand</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {medicines.map((medicine) => (
          <tr key={medicine.id} className={getRowClasses(medicine)}>
            <td>{medicine.fullName}</td>
            <td>{new Date(medicine.expiryDate).toLocaleDateString()}</td>
            <td>{medicine.quantity}</td>
            <td>{medicine.price.toFixed(2)}</td>
            <td>{medicine.brand}</td>
            <td className="actions-cell">
              <button
                disabled={medicine.quantity === 0}
                onClick={() => onSell(medicine)}
              >
                Sell
              </button>
              <button
                className="icon-button"
                title="Edit medicine"
                aria-label="Edit medicine"
                onClick={() => onEdit(medicine)}
              >
                ✏️
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
