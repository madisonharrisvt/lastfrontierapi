using System;
using System.Net;
using System.Net.Mail;
using LastFrontierApi.Models;

namespace LastFrontierApi.Helpers
{
    public class Email
    {
        public static void SendEmail(string email, Event lfEvent)
        { 
            var fromAddress = new MailAddress("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "Last Frontier Directors");
            var toAddress = new MailAddress(email, " ");
            const string fromPassword = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            const string subject = "You have been checked in";
            var body = $@"
<p>Thank you for checking in to our {lfEvent.StartDate.Month}/{lfEvent.StartDate.Day} event! </p>
<p>We know your stay in New Paradiso will be rewarding and relaxing!</p>
<p>Remember, a productive citizen is a happy citizen!</p>
<p>Connect with us on our <a href=""https://discord.gg/fETn7SW"" >Discord server</a> and like our <a href=""https://www.facebook.com/LastFrontierLARP"">Facebook page</a>!</p>
<p>Sincerely,</p>
<p>Your Last Frontier Directors</p>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}

