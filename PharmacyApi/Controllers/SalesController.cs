using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Models;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public SalesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    // GET /api/sales
    [HttpGet]
    public ActionResult<IEnumerable<SaleRecord>> GetAll()
    {
        return Ok(_medicineService.GetAllSales());
    }
}
