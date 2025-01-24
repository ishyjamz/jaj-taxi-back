using jaj_taxi_back.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jaj_taxi_back.Services
{
    public interface IBookingService
    {
        Task<ICollection<Booking>> GetBookingsAsync();
        Task<ICollection<AirportBooking>> GetAirportBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<AirportBooking?> GetAirportBookingByIdAsync(int id);
        Task<bool> CreateBookingAsync(Booking booking);
        Task<bool> CreateAirportBookingAsync(AirportBooking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> UpdateAirportBookingAsync(AirportBooking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> DeleteAirportBookingAsync(int id);
    }
}