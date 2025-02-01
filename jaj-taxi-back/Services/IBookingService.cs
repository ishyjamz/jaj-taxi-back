using jaj_taxi_back.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace jaj_taxi_back.Services
{
    public interface IBookingService
    {
        Task<ICollection<Booking>> GetBookingsAsync();
        Task<ICollection<AirportBooking>> GetAirportBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(ObjectId id);
        Task<AirportBooking?> GetAirportBookingByIdAsync(ObjectId id);
        Task<bool> CreateBookingAsync(Booking booking);
        Task<bool> CreateAirportBookingAsync(AirportBooking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> UpdateAirportBookingAsync(AirportBooking booking);
        Task<bool> DeleteBookingAsync(ObjectId id);
        Task<bool> DeleteAirportBookingAsync(ObjectId id);
    }
}