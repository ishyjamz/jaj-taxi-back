using jaj_taxi_back.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class TaxiBookingDbContext : DbContext
{
    public TaxiBookingDbContext(DbContextOptions<TaxiBookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    
    public DbSet<AirportBooking> AirportBookings { get; set; }
}