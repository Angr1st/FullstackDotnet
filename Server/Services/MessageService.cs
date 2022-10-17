using Lib.Shared;
using Lib.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;
using Server.Database;

namespace Server.Services
{
    public class MessageService : IMessageService
    {
        private readonly ConditionService _conditionService;
        private int _messageCounter = 0;
        private readonly PeriodicTimer _periodicTimer;
        private readonly ILogger _logger;
        private readonly IDbContextFactory<ServerDbContext> _dbContextFactory;


        public MessageService(ILogger<MessageService> logger, IDbContextFactory<ServerDbContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _conditionService = new ConditionService(() => Environment.Exit(0), TenMessagesSend, FortyCharactersReceived);
            _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(3));

            bool TenMessagesSend() => _messageCounter >= 10;
            bool FortyCharactersReceived()
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var messages = dbContext.UserMessages.OrderByDescending(um => um.Timestamp).ToList();
                if (messages.Count == 0)
                {
                    return false;
                }
                return messages.Select(msg => msg.Message?.Length ?? 0).Sum() >= 40;
            }
        }

        public async IAsyncEnumerable<MessageRecord> GetMessages(CallContext context = default)
        {
            while (await _periodicTimer.WaitForNextTickAsync())
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var messages = await dbContext.UserMessages.OrderByDescending(um => um.Timestamp).ToListAsync();
                if (messages.Count == 0)
                {
                    yield return new MessageRecord(DateTime.UtcNow);
                }
                else
                {
                    yield return new MessageRecord(DateTime.UtcNow, messages[0].Message);
                }
                _messageCounter++;
                _conditionService.EvaluateConditions();
            }
            yield break;
        }

        public async ValueTask SendMessage(MessageRecord userMessage, CallContext context = default)
        {
            _logger.LogInformation($"Received user input: {userMessage.Message}");
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.UserMessages.Add(new UserMessage(userMessage.Message));
            await dbContext.SaveChangesAsync();
            _conditionService.EvaluateConditions();
            await ValueTask.CompletedTask;
        }
    }
}
