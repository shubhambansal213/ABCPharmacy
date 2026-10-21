import MedicineForm from './MedicineForm';

const emptyForm = {
  fullName: '',
  notes: '',
  expiryDate: '',
  quantity: '',
  price: '',
  brand: '',
};

export default function AddMedicineForm({ onAdd }) {
  return (
    <MedicineForm
      className="add-medicine-form"
      title={<h2>Add Medicine</h2>}
      initialValues={emptyForm}
      onSubmit={onAdd}
    >
      <button type="submit">Add Medicine</button>
    </MedicineForm>
  );
}
