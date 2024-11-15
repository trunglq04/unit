using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Entities.Models;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;

namespace Unit.Service
{
    class EmailManager
    {
        static void Main(string[] args)
        {
            

            var config = new AmazonSimpleEmailServiceConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1 
            };

            using (var client = new AmazonSimpleEmailServiceClient(config))
            {
                var request = new SendEmailRequest
                {
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { "lecanhtrong113@gmail.com" }    //receive mail
                    },
                    Message = new Amazon.SimpleEmail.Model.Message
                    {
                        Body = new Body
                        {
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = "This is the body of your email"
                            }
                        },
                        Subject = new Content
                        {
                            Charset = "UTF-8",
                            Data = "Test email from SES"
                        }
                    },
                    Source = "sinh123123444@gmail.com" // Verified mail
                };

                try
                {
                    var response = client.SendEmailAsync(request);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
            }
        }
    }
}