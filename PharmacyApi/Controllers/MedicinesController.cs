using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Models;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    // GET /api/medicines?search=para
    [HttpGet]
    public ActionResult<IEnumerable<Medicine>> GetAll([FromQuery] string? search)
    {
        return Ok(_medicineService.GetAll(search));
    }

    // GET /api/medicines/5
    [HttpGet("{id:int}")]
    public ActionResult<Medicine> GetById(int id)
    {
        return Ok(_medicineService.GetById(id));
    }

    // POST /api/medicines
    [HttpPost]
    public ActionResult<Medicine> Add([FromBody] Medicine medicine)
    {
        var created = _medicineService.Add(medicine);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/medicines/5
    [HttpPut("{id:int}")]
    public ActionResult<Medicine> Update(int id, [FromBody] Medicine medicine)
    {
        var updated = _medicineService.Update(id, medicine);
        return Ok(updated);
    }

    // POST /api/medicines/5/sale
    [HttpPost("{id:int}/sale")]
    public ActionResult<SaleRecord> RecordSale(int id, [FromBody] SaleRequest request)
    {
        var sale = _medicineService.RecordSale(id, request.Quantity);
        return Ok(sale);
    }
}
