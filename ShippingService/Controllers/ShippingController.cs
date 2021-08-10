using Microsoft.AspNetCore.Mvc;

namespace ShippingService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ShippingController : ControllerBase
  {
    [HttpPost]
    [Route("ship")]
    public IActionResult Ship(ShippingModel model)
    {
      if (model.Address.Contains("China"))
      {
        return BadRequest();
      }

      return Ok();
    }
  }
}