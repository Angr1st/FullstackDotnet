using System.Runtime.Serialization;
using System.Text;

namespace Lib.Shared
{
    [DataContract]
    public record MessageRecord
    {
        [DataMember(Order = 1)]
        public string Message { get; init; }

        public MessageRecord() { }

        public MessageRecord(string message) 
        {
            Message = message;
        }

        public MessageRecord(DateTime timestamp, string message)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(timestamp.ToString("s"));
            stringBuilder.Append(" -> ");
            foreach (var item in message.Reverse())
            {
                stringBuilder.Append(item);
            }
            Message = stringBuilder.ToString();
        }

        public MessageRecord(DateTime timestamp)
        {
            Message = timestamp.ToString("s");
        }
    }
}
