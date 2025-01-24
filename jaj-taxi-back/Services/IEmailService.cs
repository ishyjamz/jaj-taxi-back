using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;

namespace jaj_taxi_back.Services
{
    public interface IEmailService
    {
        // Method to send booking confirmation emails (both customer and business)
        Task SendBookingConfirmationEmailAsync(BookingDto bookingDto);

        // Method to send airport booking confirmation emails (both customer and business)
        Task SendAirportBookingConfirmationEmailAsync(AirportBookingDto airportBookingDto);

        // Method to send booking status update emails to customer
        Task SendBookingStatusUpdateEmailAsync(BookingDto bookingDto);

        // Method to send airport booking status update emails to customer
        Task SendBookingStatusUpdateEmailAsync(AirportBookingDto airportBookingDto);
        
        Task SendContactUsEmailAsync(ContactUs contactUs);
    }
}