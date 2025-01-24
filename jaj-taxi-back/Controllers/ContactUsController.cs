using AutoMapper;
using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;
using jaj_taxi_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace jaj_taxi_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactUsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactUsController> _logger;
    private readonly IMapper _mapper;
    
    public ContactUsController(IEmailService emailService, ILogger<ContactUsController> logger, IMapper mapper)
    {
        _emailService = emailService;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> SendContactUsEmail([FromBody] ContactUsDto contactUsDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid contact request received: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var query = _mapper.Map<ContactUs>(contactUsDto);
            await _emailService.SendContactUsEmailAsync(query);
            

            return Ok(new
            {
                Message = "Booking successfully created and email sent to the business.", ContactDetails = query
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the query.");
            return StatusCode(500, "An unexpected error occurred while processing your query.");
        }
    }
}