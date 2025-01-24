using System.ComponentModel.DataAnnotations;
using jaj_taxi_back.Enums;

namespace jaj_taxi_back.Models.Entities;

public class Booking
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Time is required.")]
    [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format (HH:mm).")]
    public string Time { get; set; }

    [Required(ErrorMessage = "Pickup location is required.")]
    [StringLength(250, ErrorMessage = "Pickup location cannot exceed 250 characters.")]
    public string PickupLocation { get; set; }

    [Required(ErrorMessage = "Drop-off location is required.")]
    [StringLength(250, ErrorMessage = "Drop-off location cannot exceed 250 characters.")]
    public string DropOffLocation { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
    public string PhoneNumber { get; set; }
    
    [StringLength(250, ErrorMessage = "Cannot exceed 250 characters.")]
    public string SpecialRequests { get; set; }
    
    public Status Status { get; set; }

}
