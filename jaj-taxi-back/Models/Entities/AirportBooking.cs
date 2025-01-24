using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using jaj_taxi_back.Enums;

namespace jaj_taxi_back.Models.Entities;

public class AirportBooking
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string PickupLocation { get; set; }

    [Required]
    public string AirportName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime PickupDate { get; set; } // Only the date
    
    [Required(ErrorMessage = "Time is required.")]
    [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format (HH:mm).")]
    public string PickupTime { get; set; }

    public string? SpecialRequests { get; set; }

    [Required]
    public bool IsReturnTrip { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; } // Nullable for one-way trips
    
    [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format (HH:mm).")]
    public string? ReturnTime { get; set; }
    
    public Status Status { get; set; }
    
}
