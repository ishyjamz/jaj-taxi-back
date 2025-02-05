using jaj_taxi_back.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace jaj_taxi_back.Services;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IMongoCollection<Booking> _bookings;
    private readonly IMongoCollection<AirportBooking> _airportBookings;

    public BookingService(IMongoDatabase database, ILogger<BookingService> logger)
    {
        _bookings = database.GetCollection<Booking>("Bookings");
        _airportBookings = database.GetCollection<AirportBooking>("AirportBookings");
        _logger = logger;
    }

    public async Task<ICollection<Booking>> GetBookingsAsync()
    {
        _logger.LogInformation("Fetching all bookings.");
        return await _bookings.Find(_ => true).ToListAsync();
    }

    public async Task<ICollection<AirportBooking>> GetAirportBookingsAsync()
    {
        _logger.LogInformation("Fetching all airport bookings.");
        return await _airportBookings.Find(_ => true).ToListAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(string id)
    {
        _logger.LogInformation("Fetching booking with ID {Id}.", id);
        return await _bookings.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<AirportBooking?> GetAirportBookingByIdAsync(string id)
    {
        _logger.LogInformation("Fetching airport booking with ID {Id}.", id);
        return await _airportBookings.Find(ab => ab.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateBookingAsync(Booking booking)
    {
        try
        {
            await _bookings.InsertOneAsync(booking);
            return true;
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
            await _airportBookings.InsertOneAsync(booking);
            return true;
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
            var result = await _bookings.ReplaceOneAsync(b => b.Id == booking.Id, booking);
            return result.ModifiedCount > 0;
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
            var result = await _airportBookings.ReplaceOneAsync(ab => ab.Id == booking.Id, booking);
            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update airport booking.");
            return false;
        }
    }

    public async Task<bool> DeleteBookingAsync(string id)
    {
        try
        {
            var result = await _bookings.DeleteOneAsync(b => b.Id == id);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting booking with ID {Id}.", id);
            return false;
        }
    }

    public async Task<bool> DeleteAirportBookingAsync(string id)
    {
        try
        {
            var result = await _airportBookings.DeleteOneAsync(ab => ab.Id == id);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting airport booking with ID {Id}.", id);
            return false;
        }
    }
}