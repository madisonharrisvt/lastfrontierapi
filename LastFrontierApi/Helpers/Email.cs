using System.Net;
using System.Net.Mail;
using System.Web;
using LastFrontierApi.Models;

namespace LastFrontierApi.Helpers
{
  public class Email
  {
    private const string FromAddress = "lxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
    private const string FromPassword = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

    public static void SendCheckInEmail(string email, Event lfEvent)
    {
      var toAddress = new MailAddress(email, " ");
      const string subject = "You have been checked in";
      var body = $@"
<p>Thank you for checking in to our {lfEvent.StartDate.Month}/{lfEvent.StartDate.Day} event! </p>
<p>We know your stay in New Paradiso will be rewarding and relaxing!</p>
<p>Remember, a productive citizen is a happy citizen!</p>
<p>Connect with us on our <a href=""https://discord.gg/fETn7SW"" >Discord server</a> and like our <a href=""https://www.facebook.com/LastFrontierLARP"">Facebook page</a>!</p>
<p>Sincerely,</p>
<p>Your Last Frontier Directors</p>";

      SendEmail(toAddress, subject, body);
    }

    public static void SendPasswordResetLink(string email, string token)
    {
      var urlToken = HttpUtility.UrlEncode(token);
      var toAddress = new MailAddress(email, " ");
      const string subject = "Password reset requested";
      var body = $@"
<p>A password reset has been requested for your Last Frontier acccount.</p>
<br />
<p>If you did not make this request, please disregard this message.</p>
<br />
<p>If you did make this request, please follow the link below to continue resetting your password: </p>
<p><a href=""https://database.lastfrontierlarp.com/reset-password/{email}/{
          urlToken
        }"" >https://database.lastfrontierlarp.com/reset-password/{email}/{urlToken}</a></p>
<br />
<p>Sincerely,</p>
<p>Your Last Frontier Directors</p>";

      SendEmail(toAddress, subject, body);
    }

    public static void SendPreRegConfirmationEmail(PreRegEmailDetails details)
    {
      var toAddress = new MailAddress(details.Email, " ");
      var subject = $"Last Frontier Pre-Registration Confirmation for Event {details.Event.Title}";
      var body = $@"<!DOCTYPE html>
<html>
<head>
<style>
* {{
    font-family: Courier, sans-serif;
    color: #ffff;
    background-color: #1a2f4f;

}}
table {{
    width: 100%;
}}

td, th {{
    border: 1px solid #ccdef1;
    text-align: left;
    padding: 8px;
    line-height: 11px;
}}

tr:nth-child(even) {{
    background-color: #304c77;
}}
.order-summary-container {{
    background-color: #1a2f4f;
    width: 50%;
    margin-left: auto;
    margin-right: auto;
}}

td:last-child {{
    background-color: #1a2f4f;
    border: none;
    text-align: right;
}}

th:last-child {{
    background-color: #1a2f4f;
    border: none;
    text-align: right;
}}

.total {{
    text-align: right;
}}

.billing-info {{
    font-size: small;
    line-height: 7px;
}}
.center {{
    display: block;
    margin-left: auto;
    margin-right: auto;
}}
.impact {{
    font-family: Impact, sans-serif;
}}
.left-justify {{
    float: left;
    text-align:left!important;
}}
.billing-info {{
    border: none;
    padding-left: 0px;
}}
.width {{
    width: 60%;
}}

</style>
</head>
<body>

<img src='https://preview.ibb.co/bWYYa0/Email-Confermation-Header-Option-1-Copy.png' alt='Last Frontier' class='center' width=65%;>

<div class='order-summary-container'>
    <h1 style='font-family:Impact; font-size: 30px; color: #cc9933'>★<font color='ffff' class='impact'>Order Confirmed: ${
          details.OrderID
        }</font></h1>
    <p>Thank you for your purchase with Last Frontier.  You have been successfully pre-registered for Event: {
          details.Event.Title
        }. All XP (detailed below) has been applied to your characters.</p>

    <div class='billing-info'>
        <p style='font-family:Impact; font-size: 20px;'>BILLED TO:</p>
        <table class='width'>
            <tr>
                <td class='billing-info'>{details.BillingDetails.Name}<br></br>
                    {details.BillingDetails.AddressLine1}<br></br>
                    {details.BillingDetails.AddressCity}, {details.BillingDetails.AddressState} {
          details.BillingDetails.AddressZip
        }
                </td>
                <td class='left-justify'>
                    {details.Email}
                </td>
            </tr>
        </table>
    </div>

    <h2 style='font-family:Impact; font-size: 30px;'>Order Summary</h2>
    <table>
        <tr>
            <th>Character</th>
            <th>Base XP</th>
            <th>Vp to XP</th>
            <th>Addtl. XP</th>
            <th>Registration Cost</th> 
            <th>Addtl. XP Cost</th>
            <th>Subtotal</th>
        </tr>";


      foreach (var character in details.PreRegCharacterDetails)
      {
        var totalXp = character.BaseXp + character.CartItem.PurchaseXp + character.CartItem.VpToXp;
        var totalCost = character.BaseXpCost + character.CartItem.PurchaseXp;
        body += $@"
 <tr>
    <td>{character.Character.Name}</td>
    <td>{character.BaseXp}</td>
    <td>{character.CartItem.VpToXp}</td>
    <td>{character.CartItem.PurchaseXp}</td>
    <td>${character.BaseXpCost}.00</td>
    <td>${character.CartItem.PurchaseXp}.00</td>
    <td>{totalXp}XP ${totalCost}.00</td>
</tr>
";
      }

      body += $@"    
    </table>

    <hr>

    <p class='total'><strong>TOTAL:</strong> ${details.GrandTotal}.00</p> 

    <p><strong>Remaining VP: </strong> {details.RemainingVp} </p>
    
    <img src='https://lastfrontierlarp.com/wp-content/uploads/2018/10/Last-Frontier-Logo-Transparent.png' alt='Last Frontier' class='center' width=40%;>
</div>
</body>
</html>
";

      SendEmail(toAddress, subject, body);
    }

    private static void SendEmail(MailAddress toAddress, string subject, string body)
    {
      var fromAddress = new MailAddress(FromAddress, "Last Frontier Directors");

      var smtp = new SmtpClient
      {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
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