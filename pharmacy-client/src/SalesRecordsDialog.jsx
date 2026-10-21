export default function SalesRecordsDialog({ sales, loading, error, onClose }) {
  return (
    <div className="dialog-backdrop">
      <div className="dialog dialog-wide">
        <h3>Sale Records</h3>
        {error && <p className="form-error">{error}</p>}
        {loading ? (
          <p>Loading...</p>
        ) : sales.length === 0 ? (
          <p className="empty-message">No sales recorded yet.</p>
        ) : (
          <table className="medicine-grid">
            <thead>
              <tr>
                <th>Medicine Name</th>
                <th>Quantity Sold</th>
                <th>Unit Price</th>
                <th>Total</th>
                <th>Sale Date</th>
              </tr>
            </thead>
            <tbody>
              {sales.map((sale) => (
                <tr key={sale.id}>
                  <td>{sale.medicineName}</td>
                  <td>{sale.quantitySold}</td>
                  <td>{sale.unitPrice.toFixed(2)}</td>
                  <td>{(sale.quantitySold * sale.unitPrice).toFixed(2)}</td>
                  <td>{new Date(sale.saleDate).toLocaleString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
        <div className="dialog-actions">
          <button type="button" onClick={onClose} className="secondary">Close</button>
        </div>
      </div>
    </div>
  );
}
