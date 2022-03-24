using System.Collections.Generic;
using System.Threading.Tasks;
using BrumBrum.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BrumBrum.Consumers
{
    public class WebhookRequestFaultConsumer : IConsumer<Fault<WebhookRequest>>
    {
        private readonly ILogger<WebhookRequestFaultConsumer> _logger;

        public WebhookRequestFaultConsumer(ILogger<WebhookRequestFaultConsumer> logger) => _logger = logger;

        public Task Consume(ConsumeContext<Fault<WebhookRequest>> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, dynamic>
            {
                ["MessageId"] = context.MessageId,
                ["CorrelationId"] = context.CorrelationId,
                ["ConversationId"] = context.ConversationId,
                ["SourceAddress"] = context.SourceAddress,
            });
            
            _logger.LogWarning("[{ExceptionCount}] exception(s) occurred when attempting to send webhook commands", context.Message.Exceptions.Length);

            foreach (var exception in context.Message.Exceptions)
            {
                _logger.LogError("[{ExceptionType}]: {ExceptionMessage} \n{ExceptionSource} \n{ExceptionStackTrace}",
                    exception.ExceptionType, exception.Message, exception.Source, exception.StackTrace);
            }

            return Task.CompletedTask;
        }
    }
}