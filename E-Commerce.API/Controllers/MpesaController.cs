using E_Commerce.Application.DTOs.Mpesa;
using E_Commerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

//controller to handle mpesa callbacks
[ApiController]
[Route("api/[controller]")]
public class MpesaController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<MpesaController> _logger;

    public MpesaController(IOrderService orderService, ILogger<MpesaController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    //endpoint to receive mpesa payment callbacks
    [HttpPost("callback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MpesaCallback([FromBody] MpesaCallbackRequest request)
    {
        try
        {
            _logger.LogInformation("Received M-Pesa callback: {Callback}",
                System.Text.Json.JsonSerializer.Serialize(request));

            await _orderService.ProcessMpesaCallbackAsync(request);

            return Ok(new { ResultCode = 0, ResultDesc = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing M-Pesa callback");

            // always return 200 OK to M-Pesa to avoid retries
            return Ok(new { ResultCode = 1, ResultDesc = "Failed" });
        }
    }
}