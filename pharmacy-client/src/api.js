const API_BASE = '/api';

async function handleResponse(res) {
  if (!res.ok) {
    // Our API returns { status, message }; ASP.NET's built-in validation errors use { title }.
    // The body may also be empty or not JSON (e.g. API not running), hence the fallbacks.
    const body = await res.json().catch(() => null);
    throw new Error(body?.message ?? body?.title ?? `Request failed with status ${res.status}`);
  }
  return res.status === 204 ? null : res.json();
}

export function getMedicines(search) {
  const query = search ? `?search=${encodeURIComponent(search)}` : '';
  return fetch(`${API_BASE}/medicines${query}`).then(handleResponse);
}

export function addMedicine(medicine) {
  return fetch(`${API_BASE}/medicines`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(medicine),
  }).then(handleResponse);
}

export function updateMedicine(id, medicine) {
  return fetch(`${API_BASE}/medicines/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(medicine),
  }).then(handleResponse);
}

export function recordSale(medicineId, quantity) {
  return fetch(`${API_BASE}/medicines/${medicineId}/sale`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ quantity }),
  }).then(handleResponse);
}

export function getSales() {
  return fetch(`${API_BASE}/sales`).then(handleResponse);
}
