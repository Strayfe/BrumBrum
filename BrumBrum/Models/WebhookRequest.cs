using System;
using System.Collections.Generic;

namespace BrumBrum.Models
{
    public class WebhookRequest
    {
        public IEnumerable<Uri> Targets { get; set; }
        public dynamic Payload { get; set; }
    }
}