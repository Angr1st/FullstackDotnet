using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Database
{
    [Table("UserMessageStore")]
    public class UserMessage
    {
        public Guid UserMessageId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public UserMessage() { }
        public UserMessage(string message) 
        {
            Timestamp = DateTime.UtcNow;
            UserMessageId = Guid.NewGuid();
            Message = message;
        }
    }
}
