using AutoMapper;
using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;
using jaj_taxi_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace jaj_taxi_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public BookingController(
            IBookingService bookingService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        // General method to ensure DateTime values are in UTC
        private DateTime EnsureUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            return dateTime.Kind == DateTimeKind.Local ? dateTime.ToUniversalTime() : dateTime;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid booking request received: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                bookingDto.Date = EnsureUtc(bookingDto.Date);

                var booking = _mapper.Map<Booking>(bookingDto);
                var success = await _bookingService.CreateBookingAsync(booking);

                if (!success)
                {
                    _logger.LogWarning("Failed to save the booking to the database.");
                    return StatusCode(500, "Failed to save the booking.");
                }

                _logger.LogInformation("Booking successfully saved to the database.");

                // Send the booking confirmation emails (business and customer)
                await _emailService.SendBookingConfirmationEmailAsync(bookingDto);

                return Ok(new
                {
                    Message = "Booking successfully created and email sent to the business.", BookingDetails = booking
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the booking.");
                return StatusCode(500, "An unexpected error occurred while processing your booking.");
            }
        }

        [HttpPost("createAirport")]
        public async Task<IActionResult> CreateAirportBooking([FromBody] AirportBookingDto airportBookingDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid airport booking request received: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                airportBookingDto.PickupDate = EnsureUtc(airportBookingDto.PickupDate);
                if (airportBookingDto.ReturnDate.HasValue)
                {
                    airportBookingDto.ReturnDate = EnsureUtc(airportBookingDto.ReturnDate.Value);
                }

                var airportBooking = _mapper.Map<AirportBooking>(airportBookingDto);
                var success = await _bookingService.CreateAirportBookingAsync(airportBooking);

                if (!success)
                {
                    _logger.LogWarning("Failed to save the airport booking to the database.");
                    return StatusCode(500, "Failed to save the airport booking.");
                }

                _logger.LogInformation("Airport booking successfully saved to the database.");

                // Send the airport booking confirmation emails (business and customer)
                await _emailService.SendAirportBookingConfirmationEmailAsync(airportBookingDto);

                return Ok(new
                {
                    Message = "Airport booking successfully created and email sent to the business.",
                    BookingDetails = airportBooking
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the airport booking.");
                return StatusCode(500, "An error occurred while processing your airport booking.");
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _bookingService.GetBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving bookings.");
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

        [HttpGet("getAirport")]
        public async Task<IActionResult> GetAirportBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAirportBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving airport bookings.");
                return StatusCode(500, "An error occurred while retrieving airport bookings.");
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving booking.");
                return StatusCode(500, "An error occurred while retrieving booking.");
            }
        }

        [HttpGet("getAirport/{id}")]
        public async Task<IActionResult> GetAirportBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetAirportBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Airport booking with ID {id} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving airport booking.");
                return StatusCode(500, "An error occurred while retrieving airport booking.");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid booking update request received: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                bookingDto.Date = EnsureUtc(bookingDto.Date);

                var booking = _mapper.Map<Booking>(bookingDto);
                booking.Id = id; // Ensure the ID is set for updating
                var success = await _bookingService.UpdateBookingAsync(booking);

                if (!success)
                {
                    _logger.LogWarning("Failed to update the booking in the database.");
                    return StatusCode(500, "Failed to update the booking.");
                }

                // Send the email to the customer informing them about the booking status change
                await _emailService.SendBookingStatusUpdateEmailAsync(bookingDto);

                return Ok(new { Message = $"Airport booking status successfully updated to {bookingDto.Status.ToString()}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the booking.");
                return StatusCode(500, "An error occurred while updating the booking.");
            }
        }

        [HttpPut("updateAirport/{id}")]
        public async Task<IActionResult> UpdateAirportBooking(int id, [FromBody] AirportBookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid booking update request received: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                bookingDto.PickupDate = EnsureUtc(bookingDto.PickupDate);

                if (bookingDto.IsReturnTrip)
                    bookingDto.ReturnDate = EnsureUtc(bookingDto.ReturnDate.Value);

                var booking = _mapper.Map<AirportBooking>(bookingDto);
                booking.Id = id; // Ensure the ID is set for updating
                var success = await _bookingService.UpdateAirportBookingAsync(booking);

                if (!success)
                {
                    _logger.LogWarning("Failed to update the booking in the database.");
                    return StatusCode(500, "Failed to update the booking.");
                }
                
                // Send the email to the customer informing them about the booking status change
                await _emailService.SendBookingStatusUpdateEmailAsync(bookingDto);

                return Ok(new { Message = $"Airport booking status successfully updated to {bookingDto.Status.ToString()}." });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the booking.");
                return StatusCode(500, "An error occurred while updating the booking.");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                // Attempt to delete the booking
                var success = await _bookingService.DeleteBookingAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Booking with ID {Id} not found or could not be deleted.", id);
                    return NotFound(new { Message = $"Booking with ID {id} not found or could not be deleted." });
                }

                _logger.LogInformation("Booking with ID {Id} successfully deleted.", id);
                return Ok(new { Message = "Booking successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the booking with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while deleting the booking." });
            }
        }

        [HttpDelete("deleteAirport/{id}")]
        public async Task<IActionResult> DeleteAirportBooking(int id)
        {
            try
            {
                // Attempt to delete the booking
                var success = await _bookingService.DeleteAirportBookingAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Booking with ID {Id} not found or could not be deleted.", id);
                    return NotFound(new { Message = $"Booking with ID {id} not found or could not be deleted." });
                }

                _logger.LogInformation("Booking with ID {Id} successfully deleted.", id);
                return Ok(new { Message = "Booking successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the booking with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while deleting the booking." });
            }
        }
    }
}