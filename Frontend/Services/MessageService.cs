using Lib.Shared;
using Lib.Shared.Interfaces;
using ProtoBuf.Grpc;

namespace Frontend.Services
{
    public class MessageService
    {
        private readonly IMessageService _messageService;

        public MessageService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async IAsyncEnumerable<MessageRecord> GetMessages(CallContext context = default)
        {
            await foreach (var message in _messageService.GetMessages(context))
            {
                yield return message;
            }
            yield break;
        }

        public async ValueTask SendMessage(
           MessageRecord userMessage,
            CallContext context = default)
        {
           await _messageService.SendMessage(userMessage, context);
        }
    }
}
