using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrumBrum.Commands;
using BrumBrum.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BrumBrum.Consumers
{
    public class WebhookRequestConsumer : IConsumer<WebhookRequest>
    {
        private readonly ILogger<WebhookRequestConsumer> _logger;

        public WebhookRequestConsumer(ILogger<WebhookRequestConsumer> logger) => _logger = logger;
        
        public async Task Consume(ConsumeContext<WebhookRequest> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, dynamic>
            {
                ["MessageId"] = context.MessageId,
                ["CorrelationId"] = context.CorrelationId,
                ["ConversationId"] = context.ConversationId,
                ["SourceAddress"] = context.SourceAddress,
            });

            var tasks = context.Message.Targets
                .Select(target => context.Send(new WebhookCommand {Target = target, Payload = context.Message.Payload}))
                .ToArray();

            await Task.WhenAll(tasks);
        }
    }
}