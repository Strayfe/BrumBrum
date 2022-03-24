namespace BrumBrum.Configuration
{
    public class QueueConfiguration
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public string SendEndpoint { get; set; }
    }
}