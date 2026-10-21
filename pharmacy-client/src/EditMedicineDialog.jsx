import MedicineForm from './MedicineForm';

export default function EditMedicineDialog({ medicine, onConfirm, onCancel }) {
  const initialValues = {
    fullName: medicine.fullName,
    notes: medicine.notes ?? '',
    expiryDate: medicine.expiryDate.slice(0, 10),
    quantity: medicine.quantity,
    price: medicine.price,
    brand: medicine.brand,
  };

  return (
    <div className="dialog-backdrop">
      <MedicineForm
        className="dialog dialog-wide"
        title={<h3>Edit {medicine.fullName}</h3>}
        initialValues={initialValues}
        onSubmit={(updated) => onConfirm(medicine.id, updated)}
      >
        <div className="dialog-actions">
          <button type="submit">Save Changes</button>
          <button type="button" onClick={onCancel} className="secondary">Cancel</button>
        </div>
      </MedicineForm>
    </div>
  );
}
