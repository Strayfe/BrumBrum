using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BrumBrum.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BrumBrum.Consumers
{
    public class WebhookConsumer : IConsumer<WebhookCommand>
    {
        private readonly ILogger<WebhookConsumer> _logger;
        private readonly HttpClient _httpClient;

        public WebhookConsumer(ILogger<WebhookConsumer> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task Consume(ConsumeContext<WebhookCommand> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, dynamic>
            {
                ["MessageId"] = context.MessageId,
                ["CorrelationId"] = context.CorrelationId,
                ["ConversationId"] = context.ConversationId,
                ["SourceAddress"] = context.SourceAddress,
            });
            
            _logger.LogDebug("Sending webhook to [{Target}]", context.Message.Target);

            var response = await _httpClient.GetAsync(context.Message.Target);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Response received [{StatusCode}], with body [{Body}]", response.StatusCode, content);
        }
    }
}