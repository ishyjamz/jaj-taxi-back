using System.ComponentModel.DataAnnotations;

namespace jaj_taxi_back.Models.Entities;

public class ContactUs
{
    public int Id { get; set; }
    
    [Required]
    [MinLength(3)] // Minimum length of 3 characters
    [MaxLength(50)] // Maximum length of 50 characters
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Message { get; set; }
}