using Kolokwium1Poprawka.Exceptions;
using Kolokwium1Poprawka.Models_DTOs;
using Kolokwium1Poprawka.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1Poprawka.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("{idClient}")]
    public async Task<IActionResult> GetClient(int idClient)
    {
        try
        {
            var result = await _clientService.GetClientAsync(idClient);
            return Ok(result);
        }
        catch (ClientNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }


    [HttpPost]
    public async Task<IActionResult> PostClientAndRent([FromBody] CreateClientAndRentDTO createClientAndRentDto)
    {
        try
        {
            await _clientService.PostClientAndRentAsync(createClientAndRentDto);
            return Created();
        }
        catch (CarNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}