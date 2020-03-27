using Domain.Interfaces.BroadcastServer;

namespace iBank.BroadcastServer.Email
{
    public class WorkingEmailSection : IEmailSection
    {
        public string Html { get; set; }

        public string Text { get; set; }
    }
}
