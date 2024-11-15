
namespace Unit.Shared.DataTransferObjects
{
    public class EmailRequestDto
    {
        public string SenderEmail { get; set; }       // Sender's email
        public string RecipientEmail { get; set; }    // Recipient's email
        public string Subject { get; set; }           // Email subject
        public string Body { get; set; }              // Email body content
    }
}
