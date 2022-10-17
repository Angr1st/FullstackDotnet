using ProtoBuf.Grpc;
using System.ServiceModel;

namespace Lib.Shared.Interfaces
{
    /// <summary>
    /// This Interface is for the Service for exchanging messages between client and server
    /// </summary>
    [ServiceContract]
    public interface IMessageService
    {
        /// <summary>
        /// Server-Side streaming of messages. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Message with the format YYYY-MM-DDTHH:MM:SS</returns>
        IAsyncEnumerable<MessageRecord> GetMessages(CallContext context = default);
        /// <summary>
        /// Client-Side sending of user input messages.
        /// </summary>
        /// <param name="userMessages"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        ValueTask SendMessage(MessageRecord userMessage, CallContext context = default);
    }
}
