using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace ST10361554_CLDV6211_Order_Notification_Email_Sender
{
    public static class EmailSender
    {
		// Code Attribution
		// Method written using code from: 
		// cgillum
		// Microsoft Learn
		// https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp-inproc

		[FunctionName("EmailSender")]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
             ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                OrderEventData data = JsonConvert.DeserializeObject<OrderEventData>(requestBody);

                await SendEmailNotification(data); // Call the email sending method

                log.LogInformation($"Email sent successfully to {data.UserEmail} for Order: {data.OrderId} on Date: {data.NotificationDate}");

                //Console.WriteLine($"Email sent successfully to {data.UserEmail} for Order: {data.OrderId} on Date: {data.NotificationDate}");

                return new OkObjectResult($"Email sent successfully to {data.UserEmail} for Order: {data.OrderId} on Date: {data.NotificationDate}");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to send email: {ex.Message}");

                //Console.WriteLine($"Failed to send email: {ex.Message}");

                return new BadRequestObjectResult("Failed to send email.");
            }
        }

		// Code Attribution
		// Method written using code from: 
		// Thomas Ardal
		// elmah.io
		// https://blog.elmah.io/how-to-send-emails-from-csharp-net-the-definitive-tutorial/#:~:text=Sending%20emails%20from%20C%23%20using,subject%22%2C%20%22body%22)%3B

		private static async Task SendEmailNotification(OrderEventData data)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("khumalocraft123@gmail.com", "dnyf dzuk bvtn cnge");
                smtpClient.EnableSsl = true;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("khumalocraft123@gmail.com", "James Khumalo");
                    mailMessage.Subject = $"Order Status Update for Order ID: {data.OrderId} - {data.NotificationDate}";
                    mailMessage.Body = $"Notification Date: {data.NotificationDate} \nYour order {data.OrderId} status is now {data.Status}.";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(new MailAddress(data.UserEmail));

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
