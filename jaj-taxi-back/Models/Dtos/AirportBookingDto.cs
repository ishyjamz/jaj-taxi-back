using System.ComponentModel.DataAnnotations;
using jaj_taxi_back.Enums;

namespace jaj_taxi_back.Models.Dtos;

public class AirportBookingDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Pickup location is required.")]
    public string? PickupLocation { get; set; }

    [Required(ErrorMessage = "Airport name is required.")]
    public string? AirportName { get; set; }

    [Required(ErrorMessage = "Pickup date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateTime PickupDate { get; set; } // Only the date

    [Required(ErrorMessage = "Pickup time is required.")]
    [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format (HH:mm).")]
    public string? PickupTime { get; set; }

    public string? SpecialRequests { get; set; }

    [Required(ErrorMessage = "IsReturnTrip must be specified.")]
    public bool IsReturnTrip { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateTime? ReturnDate { get; set; } // Nullable for one-way trips

    [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format (HH:mm).")]
    public string? ReturnTime { get; set; }

    [Required]
    public Status Status { get; set; } = Status.Pending; // Default value
}