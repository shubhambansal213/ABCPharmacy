import { useState } from 'react';

// Shared fields, validation and payload for adding and editing a medicine.
// The caller supplies the form's class, title and action buttons (as children).
export default function MedicineForm({ className, title, initialValues, onSubmit, children }) {
  const [form, setForm] = useState(initialValues);
  const [error, setError] = useState('');

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');

    if (!form.fullName || !form.expiryDate) {
      setError('Full name and expiry date are required.');
      return;
    }

    try {
      await onSubmit({
        fullName: form.fullName,
        notes: form.notes,
        expiryDate: form.expiryDate,
        quantity: Number(form.quantity) || 0,
        price: Number(form.price) || 0,
        brand: form.brand,
      });
      setForm(initialValues);
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <form className={className} onSubmit={handleSubmit}>
      {title}
      {error && <p className="form-error">{error}</p>}
      <div className="form-grid">
        <label>
          Full Name
          <input name="fullName" value={form.fullName} onChange={handleChange} required />
        </label>
        <label>
          Brand
          <input name="brand" value={form.brand} onChange={handleChange} />
        </label>
        <label>
          Expiry Date
          <input type="date" name="expiryDate" value={form.expiryDate} onChange={handleChange} required />
        </label>
        <label>
          Quantity
          <input type="number" min="0" name="quantity" value={form.quantity} onChange={handleChange} />
        </label>
        <label>
          Price
          <input type="number" min="0" step="0.01" name="price" value={form.price} onChange={handleChange} />
        </label>
        <label className="notes-field">
          Notes
          <textarea name="notes" value={form.notes} onChange={handleChange} />
        </label>
      </div>
      {children}
    </form>
  );
}
