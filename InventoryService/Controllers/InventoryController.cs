using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class InventoryController : ControllerBase
  {
    [HttpPost]
    [Route("export")]
    public IActionResult Export(InventoryModel model)
    {
      if (model.Quantity < 1)
      {
        return BadRequest();
      }

      return Ok();
    }
  }
}