using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using jaj_taxi_back.Enums;
using jaj_taxi_back.Models.Dtos;
using jaj_taxi_back.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace jaj_taxi_back.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly string _businessEmail;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()
                             ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _businessEmail = configuration.GetValue<string>("BusinessEmailSettings:Address")
                             ?? throw new ArgumentNullException("Business email is not configured.");
        }

        public async Task SendBookingConfirmationEmailAsync(BookingDto bookingDto)
        {
            var customerEmailBody = GetCustomerEmailBody(bookingDto);
            var businessEmailBody = GetBusinessEmailBody(bookingDto);
            await SendEmailsAsync(bookingDto.Email, customerEmailBody, businessEmailBody);
        }

        public async Task SendAirportBookingConfirmationEmailAsync(AirportBookingDto airportBookingDto)
        {
            var customerEmailBody = GetCustomerEmailBody(airportBookingDto);
            var businessEmailBody = GetBusinessEmailBody(airportBookingDto);
            await SendEmailsAsync(airportBookingDto.Email, customerEmailBody, businessEmailBody);
        }

        // Method to send both customer and business emails
        private async Task SendEmailsAsync(string recipientEmail, string customerEmailBody, string businessEmailBody)
        {
            ValidateEmailAddresses(recipientEmail);

            var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);

                // Send to customer
                await SendEmail(client, recipientEmail, "Booking Confirmation", customerEmailBody);

                // Send to business
                await SendEmail(client, _businessEmail, "New Booking Alert", businessEmailBody);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error sending email.", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        // Helper method to send email
        private async Task SendEmail(SmtpClient client, string recipient, string subject, string body)
        {
            var emailMessage = new MimeMessage
            {
                From = { new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail) },
                To = { MailboxAddress.Parse(recipient) },
                Subject = subject,
                Body = new BodyBuilder
                {
                    HtmlBody = body,
                    TextBody = "This email contains HTML content. Please view it in an HTML-compatible email client."
                }.ToMessageBody()
            };
            await client.SendAsync(emailMessage);
        }

        // Helper method to validate email addresses
        private void ValidateEmailAddresses(string recipientEmail)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
                throw new ArgumentNullException(nameof(recipientEmail), "Recipient email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                throw new ArgumentNullException(nameof(_emailSettings.SenderEmail), "Sender email is not configured.");

            if (string.IsNullOrWhiteSpace(_businessEmail))
                throw new ArgumentNullException(nameof(_businessEmail), "Business email is not configured.");
        }

        // Helper methods to generate the email bodies for booking confirmation

        private string GetCustomerEmailBody(BookingDto bookingDto) => $@"
            <h2>Booking Confirmation</h2>
            <p>Dear {bookingDto.Name},</p>
            <p>Your booking is confirmed for {bookingDto.Date:yyyy-MM-dd} at {bookingDto.Time}.</p>
            <p>Pickup: {bookingDto.PickupLocation}</p>
            <p>Drop-off: {bookingDto.DropOffLocation}</p>
            <p>Thank you for choosing Jaj Taxi!</p>";

        private string GetBusinessEmailBody(BookingDto bookingDto) => $@"
            <h1>New Booking Alert</h1>
            <p>A new booking has been made:</p>
            <ul>
                <li>Name: {bookingDto.Name}</li>
                <li>Email: {bookingDto.Email}</li>
                <li>Pickup: {bookingDto.PickupLocation}</li>
                <li>Drop-off: {bookingDto.DropOffLocation}</li>
                <li>Date: {bookingDto.Date:yyyy-MM-dd}</li>
                <li>Time: {bookingDto.Time}</li>
            </ul>";

        private string GetCustomerEmailBody(AirportBookingDto airportBookingDto) => $@"
            <h2>Airport Booking Confirmation</h2>
            <p>Dear {airportBookingDto.Name},</p>
            <p>Your airport booking is confirmed for {airportBookingDto.PickupDate:yyyy-MM-dd} at {airportBookingDto.PickupTime}.</p>
            <p>Pickup: {airportBookingDto.PickupLocation}</p>
            <p>Destination: {airportBookingDto.AirportName}</p>
            {GetReturnDetails(airportBookingDto)}
            <p>Thank you for choosing Jaj Taxi!</p>";

        private string GetBusinessEmailBody(AirportBookingDto airportBookingDto) => $@"
            <h1>New Airport Booking Alert</h1>
            <p>A new airport booking has been made:</p>
            <ul>
                <li>Name: {airportBookingDto.Name}</li>
                <li>Email: {airportBookingDto.Email}</li>
                <li>Pickup: {airportBookingDto.PickupLocation}</li>
                <li>Airport: {airportBookingDto.AirportName}</li>
                <li>Pickup Date: {airportBookingDto.PickupDate:yyyy-MM-dd}</li>
                <li>Pickup Time: {airportBookingDto.PickupTime}</li>
                {GetReturnDetails(airportBookingDto)}
            </ul>";

        // Method to get return trip details
        private string GetReturnDetails(AirportBookingDto airportBookingDto)
        {
            if (!airportBookingDto.IsReturnTrip || airportBookingDto.ReturnDate == null ||
                string.IsNullOrEmpty(airportBookingDto.ReturnTime))
                return "<p>No return trip details provided.</p>";

            return $@"
                <p>Return Date: {airportBookingDto.ReturnDate:yyyy-MM-dd}</p>
                <p>Return Time: {airportBookingDto.ReturnTime}</p>";
        }

        // Send status update email to customer and business
        public async Task SendBookingStatusUpdateEmailAsync(BookingDto bookingDto)
        {
            var statusMessage = bookingDto.Status switch
            {
                Status.Accepted => "Your booking has been accepted.",
                Status.Declined => "Unfortunately, your booking has been declined.",
                _ => "Your booking status has been updated."
            };

            var customerEmailBody = GetStatusUpdateEmailBody(bookingDto, statusMessage);
            var businessEmailBody = GetStatusUpdateEmailBodyForBusiness(bookingDto, statusMessage);

            await SendEmailsAsync(bookingDto.Email, customerEmailBody, businessEmailBody);
        }

        public async Task SendBookingStatusUpdateEmailAsync(AirportBookingDto airportBookingDto)
        {
            if (airportBookingDto.Status == Status.Accepted || airportBookingDto.Status == Status.Declined)
            {
                var statusMessage = airportBookingDto.Status switch
                {
                    Status.Accepted => "Booking Accepted.",
                    Status.Declined => "Booking Declined.",
                };

                var customerEmailBody = GetStatusUpdateEmailBody(airportBookingDto, statusMessage);
                var businessEmailBody = GetStatusUpdateEmailBodyForBusiness(airportBookingDto, statusMessage);

                await SendEmailsAsync(airportBookingDto.Email, customerEmailBody, businessEmailBody);
            }
        }

        public async Task SendContactUsEmailAsync(ContactUs contactUs)
        {
            var customerEmailBody = GetContactUsEmailBodyForCustomer(contactUs);
            var businessEmailBody = GetContactUsEmailBodyForBusiness(contactUs);

            await SendEmailsAsync(contactUs.Email, customerEmailBody, businessEmailBody);
        }

        // Helper method to create status update email body for customer
        private string GetStatusUpdateEmailBody(BookingDto bookingDto, string statusMessage) => $@"
            <h2>Booking Status Update</h2>
            <p>Dear {bookingDto.Name},</p>
            <p>{statusMessage}</p>
            <p>Pickup: {bookingDto.PickupLocation}</p>
            <p>Drop-off: {bookingDto.DropOffLocation}</p>
            <p>Pickup Date: {bookingDto.Date:yyyy-MM-dd}</p>
            <p>Pickup Time: {bookingDto.Time}</p>
            <p>Thank you for choosing Jaj Taxi!</p>";

        // Helper method to create status update email body for business
        private string GetStatusUpdateEmailBodyForBusiness(BookingDto bookingDto, string statusMessage) => $@"
            <h1>Booking Status Update</h1>
            <p>{statusMessage}</p>
            <ul>
                <li>Name: {bookingDto.Name}</li>
                <li>Email: {bookingDto.Email}</li>
                <li>Pickup: {bookingDto.PickupLocation}</li>
                <li>Drop-off: {bookingDto.DropOffLocation}</li>
                <li>Date: {bookingDto.Date:yyyy-MM-dd}</li>
                <li>Time: {bookingDto.Time}</li>
            </ul>";

        private string GetStatusUpdateEmailBody(AirportBookingDto airportBookingDto, string statusMessage) => $@"
            <h2>Booking Status Update</h2>
            <p>Dear {airportBookingDto.Name},</p>
            <p>{statusMessage}</p>
            <p>Pickup: {airportBookingDto.PickupLocation}</p>
            <p>Drop-off: {airportBookingDto.AirportName}</p>
            <p>Pickup Date: {airportBookingDto.PickupDate:dd-MM-yyyy}</p>
            <p>Pickup Time: {airportBookingDto.PickupTime}</p>
            <p>{GetReturnDetails(airportBookingDto)}</p>
            <p>Thank you for choosing JAJ Taxi!</p>";

        // Helper method to create status update email body for business
        private string GetStatusUpdateEmailBodyForBusiness(AirportBookingDto airportBookingDto, string statusMessage) =>
            $@"
            <h1>Booking Status Update</h1>
            <p>{statusMessage}</p>
            <ul>
                <li>Name: {airportBookingDto.Name}</li>
                <li>Email: {airportBookingDto.Email}</li>
                <li>Pickup: {airportBookingDto.PickupLocation}</li>
                <li>Drop-off: {airportBookingDto.AirportName}</li>
                <li>Date: {airportBookingDto.PickupDate:yyyy-MM-dd}</li>
                <li>Time: {airportBookingDto.PickupTime}</li>
                <li>{GetReturnDetails(airportBookingDto)}</li>
            </ul>";

        // Helper method to create status update email body for customer
        private string GetContactUsEmailBodyForCustomer(ContactUs contactUs) => $@"
            <h2>Message Received</h2>
            <p>Dear {contactUs.Name},</p>
            <p>We are sending this email to confirm receipt of your query. We will get back to you as soon as possible.</p>
            <p>Here are the details of your query:</p>
            <ul>
                <li> Name: {contactUs.Name}</li>
                <li> Email: {contactUs.Email}</li>
                <li> Message: {contactUs.Message}</li>
            </ul>
            <p>Best regards,<br>Team JAJ Taxi</>";

        private string GetContactUsEmailBodyForBusiness(ContactUs contactUs) => $@"
            <h2>Customer Query Received:</h2>
            <p>You have received a new message from {contactUs.Name}</p>
            <p>Details:</p>
            <ul>
                <li> Name: {contactUs.Name}</li>
                <li> Email: {contactUs.Email}</li>
                <li> Message: {contactUs.Message}</li>
            </ul>";
    }
}