// Path: API/Controllers/SendingEmailController.cs
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unit.Shared.DataTransferObjects;

namespace Unit.API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class SendingEmailController : ControllerBase
    {
        private readonly IAmazonSimpleEmailService _sesClient;

        public SendingEmailController()
        {
            // Initialize the AWS SES client with the desired region
            _sesClient = new AmazonSimpleEmailServiceClient(RegionEndpoint.APSoutheast1); // Change region as needed
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            // Build the SES email request
            var emailRequest = new SendEmailRequest
            {
                Source = request.SenderEmail,  // Sender's email address
                Destination = new Destination
                {
                    ToAddresses = new List<string> { request.RecipientEmail }  // Recipient's email address
                },
                Message = new Message
                {
                    Subject = new Content
                    {
                        Charset = "UTF-8",
                        Data = request.Subject  // Email subject
                    },
                    Body = new Body
                    {
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = request.Body   // Email body content
                        }
                    }
                }
            };

            try
            {
                // Send the email using AWS SES
                var response = await _sesClient.SendEmailAsync(emailRequest);

                // Check if the email was sent successfully
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok("Email sent successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to send email.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine("Error sending email: " + ex.Message);
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }
    }
}
