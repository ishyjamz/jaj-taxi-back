using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace jaj_taxi_back.Models.Dtos;

public class ContactUsDto
{
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