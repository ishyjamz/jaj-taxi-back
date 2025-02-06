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
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return null;
        }
        
        _logger.LogInformation("Fetching booking with ID {Id}.", id);
        return await _bookings.Find(ab => ab.Id == objectId.ToString()).FirstOrDefaultAsync();
    }

    public async Task<AirportBooking?> GetAirportBookingByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return null;
        }
        
        _logger.LogInformation("Fetching airport booking with ID {Id}.", id);
        return await _airportBookings.Find(ab => ab.Id == objectId.ToString()).FirstOrDefaultAsync();
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
            if (string.IsNullOrEmpty(booking.Id))
            {
                booking.Id = ObjectId.GenerateNewId().ToString(); // Ensure MongoDB has a valid Id
            }

            await _airportBookings.InsertOneAsync(booking);
            return true; // Return the generated Id
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create airport booking.");
            return false; // Indicate failure
        }
    }


    public async Task<bool> UpdateBookingAsync(Booking booking)
    {
        try
        {
            if (!ObjectId.TryParse(booking.Id, out var objectId))
            {
                _logger.LogWarning("Invalid Booking ID: {Id}", booking.Id);
                return false;
            }

            var result = await _bookings.ReplaceOneAsync(b => b.Id == objectId.ToString(), booking);
        
            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("No booking found with ID {Id}.", booking.Id);
                return false;
            }

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update booking with ID {Id}.", booking.Id);
            return false;
        }
    }


    public async Task<bool> UpdateAirportBookingAsync(AirportBooking booking)
    {
        try
        {
            if (!ObjectId.TryParse(booking.Id, out var objectId))
            {
                _logger.LogWarning("Invalid Airport Booking ID: {Id}", booking.Id);
                return false;
            }

            var result = await _airportBookings.ReplaceOneAsync(ab => ab.Id == objectId.ToString(), booking);

            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("No airport booking found with ID {Id}.", booking.Id);
                return false;
            }

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update airport booking with ID {Id}.", booking.Id);
            return false;
        }
    }


    public async Task<bool> DeleteBookingAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                _logger.LogWarning("Invalid Booking ID: {Id}", id);
                return false;
            }

            var result = await _bookings.DeleteOneAsync(b => b.Id == objectId.ToString());

            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("No booking found with ID {Id}.", id);
                return false;
            }

            return true;
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
            if (!ObjectId.TryParse(id, out var objectId))
            {
                _logger.LogWarning("Invalid Airport Booking ID: {Id}", id);
                return false;
            }

            var result = await _airportBookings.DeleteOneAsync(ab => ab.Id == objectId.ToString());

            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("No airport booking found with ID {Id}.", id);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting airport booking with ID {Id}.", id);
            return false;
        }
    }

}