using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace jaj_taxi_back.Services;

public class BookingService : IBookingService
{
    private readonly TaxiBookingDbContext _dbContext;
    private readonly ILogger<BookingService> _logger;

    public BookingService(TaxiBookingDbContext dbContext, ILogger<BookingService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ICollection<Booking>> GetBookingsAsync()
    {
        _logger.LogInformation("Fetching all bookings.");
        return await _dbContext.Bookings.OrderBy(b => b.Id).ToListAsync();
    }

    public async Task<ICollection<AirportBooking>> GetAirportBookingsAsync()
    {
        _logger.LogInformation("Fetching all airport bookings.");
        return await _dbContext.AirportBookings.OrderBy(b => b.Id).ToListAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        _logger.LogInformation("Fetching booking with ID {Id}.", id);
        return await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<AirportBooking?> GetAirportBookingByIdAsync(int id)
    {
        _logger.LogInformation("Fetching airport booking with ID {Id}.", id);
        return await _dbContext.AirportBookings.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> CreateBookingAsync(Booking booking)
    {
        try
        {
            await _dbContext.Bookings.AddAsync(booking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create booking.");
            return false;
        }
    }

    public async Task<bool> CreateAirportBookingAsync(AirportBooking booking)
    {
        try
        {
            await _dbContext.AirportBookings.AddAsync(booking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create airport booking.");
            return false;
        }
    }

    public async Task<bool> UpdateBookingAsync(Booking booking)
    {
        try
        {
            _dbContext.Bookings.Update(booking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update booking.");
            return false;
        }
    }

    public async Task<bool> UpdateAirportBookingAsync(AirportBooking booking)
    {
        try
        {
            _dbContext.AirportBookings.Update(booking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update airport booking.");
            return false;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        try
        {
            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found.", id);
                return false;
            }

            _dbContext.Bookings.Remove(booking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting booking with ID {Id}.", id);
            return false;
        }
    }

    public async Task<bool> DeleteAirportBookingAsync(int id)
    {
        try
        {
            var airportBooking = await _dbContext.AirportBookings.FindAsync(id);
            if (airportBooking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found.", id);
                return false;
            }

            _dbContext.AirportBookings.Remove(airportBooking);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting booking with ID {Id}.", id);
            return false;
        }
    }

    private async Task<bool> SaveAsync()
    {
        try
        {
            var changes = await _dbContext.SaveChangesAsync();
            return changes > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to the database.");
            return false;
        }
    }
}
