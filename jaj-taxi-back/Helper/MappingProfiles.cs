using AutoMapper;
using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;

namespace jaj_taxi_back.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Define the mapping between BookingDto and Booking
        CreateMap<BookingDto, Booking>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.PickupLocation))
            .ForMember(dest => dest.DropOffLocation, opt => opt.MapFrom(src => src.DropOffLocation))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.SpecialRequests, opt => opt.MapFrom(src => src.SpecialRequests))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        
        // Map from AirportBookingDto to AirportBooking
        CreateMap<AirportBookingDto, AirportBooking>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.PickupLocation))
            .ForMember(dest => dest.AirportName, opt => opt.MapFrom(src => src.AirportName))
            .ForMember(dest => dest.PickupDate, opt => opt.MapFrom(src => src.PickupDate))
            .ForMember(dest => dest.PickupTime, opt => opt.MapFrom(src => src.PickupTime))
            .ForMember(dest => dest.SpecialRequests, opt => opt.MapFrom(src => src.SpecialRequests))
            .ForMember(dest => dest.IsReturnTrip, opt => opt.MapFrom(src => src.IsReturnTrip))
            .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
            .ForMember(dest => dest.ReturnTime, opt => opt.MapFrom(src => src.ReturnTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<ContactUsDto, ContactUs>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
    }
}