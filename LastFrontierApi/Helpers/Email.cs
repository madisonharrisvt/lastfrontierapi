using System;
using System.Net;
using System.Net.Mail;
using System.Web;
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

        public static void SendPasswordResetLink(string email, string token)
        {
            var urlToken = HttpUtility.UrlEncode(token);
            var fromAddress = new MailAddress("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "Last Frontier Directors");
            var toAddress = new MailAddress(email, " ");
            const string fromPassword = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            const string subject = "Password reset requested";
            var body = $@"
<p>A password reset has been requested for your Last Frontier acccount.</p>
<br />
<p>If you did not make this request, please disregard this message.</p>
<br />
<p>If you did make this request, please follow the link below to continue resetting your password: </p>
<p><a href=""http://lastfrontierlarppoc.s3-website-us-west-1.amazonaws.com/reset-password/{email}/{urlToken}"" >http://lastfrontierlarppoc.s3-website-us-west-1.amazonaws.com/reset-password/{email}/{urlToken}</a></p>
<br />
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

