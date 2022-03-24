using System;

namespace BrumBrum.Commands
{
    public class WebhookCommand
    {
        public Uri Target { get; set; }
        public dynamic Payload { get; set; }
    }
}