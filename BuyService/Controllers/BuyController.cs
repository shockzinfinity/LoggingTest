using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BuyService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BuyController : ControllerBase
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BuyController> _logger;

    public BuyController(IHttpClientFactory httpClientFactory, ILogger<BuyController> logger)
    {
      _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Buy(OrderedProduct model)
    {
      try
      {
        _logger.LogInformation("Start calling Buy function");
        var inventoryHttpClient = _httpClientFactory.CreateClient();
        inventoryHttpClient.BaseAddress = new Uri("https://localhost:5103/api/");
        inventoryHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string exportJsonRequest = "{\"productId\":\"" + model.ProductId + "\"," + "\"quantity\":" + model.Quantity + "}";

        var request = new StringContent(exportJsonRequest, Encoding.UTF8, mediaType: "application/json");
        var response = inventoryHttpClient.PostAsync(requestUri: "inventory/export", request).Result;

        var shippingHttpClient = _httpClientFactory.CreateClient();
        shippingHttpClient.BaseAddress = new Uri("https://localhost:5105/api/");
        shippingHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string jsonRequest = "{\"address\":\"" + model.Address + "\"," +
                            "\"sender\":\"" + model.OrderName + "\"," +
                            "\"receiver\":\"" + model.Receiver + "\"," +
                            "\"productName\":\"" + model.ProductName + "\"}";
        var request1 = new StringContent(jsonRequest, Encoding.UTF8, mediaType: "application/json");
        var response1 = shippingHttpClient.PostAsync(requestUri: "shipping/ship", request1).Result;

        if (response1.IsSuccessStatusCode && response.IsSuccessStatusCode)
        {
          _logger.LogInformation("End calling Buy function");
          return Ok();
        }
      }
      catch
      {
        _logger.LogError("There is an exception");
        return StatusCode(500);
      }

      return BadRequest();
    }
  }
}