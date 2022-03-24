using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrumBrum.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrumBrum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IngressController : ControllerBase
    {
        private readonly ILogger<IngressController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public IngressController(ILogger<IngressController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> PublishWebhookAsync([FromBody] WebhookRequest webhookRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var scope = _logger.BeginScope(new Dictionary<string, dynamic>
            {
                ["Action"] = nameof(PublishWebhookAsync)
            });

            try
            {
                await _publishEndpoint.Publish(webhookRequest);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred when trying to publish the webhook request");
                return BadRequest();
            }
            
            _logger.LogDebug("Published webhook successfully");
            return Ok(webhookRequest);
        }
    }
}