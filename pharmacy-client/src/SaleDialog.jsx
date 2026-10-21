import { useState } from 'react';

export default function SaleDialog({ medicine, onConfirm, onCancel }) {
  const [quantity, setQuantity] = useState(1);
  const [error, setError] = useState('');

  async function handleConfirm() {
    const qty = Number(quantity);
    if (!qty || qty <= 0) {
      setError('Enter a quantity greater than zero.');
      return;
    }
    if (!Number.isInteger(qty)) {
      setError('Enter a whole number.');
      return;
    }
    if (qty > medicine.quantity) {
      setError(`Only ${medicine.quantity} in stock.`);
      return;
    }

    try {
      await onConfirm(medicine.id, qty);
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <div className="dialog-backdrop">
      <div className="dialog">
        <h3>Sell {medicine.fullName}</h3>
        <p>Available stock: {medicine.quantity}</p>
        {error && <p className="form-error">{error}</p>}
        <label>
          Quantity to sell
          <input
            type="number"
            min="1"
            max={medicine.quantity}
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
          />
        </label>
        <div className="dialog-actions">
          <button onClick={handleConfirm}>Confirm Sale</button>
          <button onClick={onCancel} className="secondary">Cancel</button>
        </div>
      </div>
    </div>
  );
}
