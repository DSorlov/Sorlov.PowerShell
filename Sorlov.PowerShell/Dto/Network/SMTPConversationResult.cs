using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Dto.Network
{
    public class SMTPConversationResult
    {
        private string message;
        private bool success;
        private TimeSpan duration;
        private string sender;
        private string recipient;
        private string mailserver;
        public string Message { get { return message; } }
        public bool Success { get { return success; } }
        public TimeSpan Duration { get { return duration; } }
        public string Mailserver { get { return mailserver; } }
        public string Sender { get { return sender; } }
        public string Recipient { get { return recipient; } }

        public SMTPConversationResult(string Message, bool Success, TimeSpan Duration, string Sender, string Recipient, string Mailserver)
        {
            message = Message;
            success = Success;
            duration = Duration;
            mailserver = Mailserver;
            sender = Sender;
            recipient = Recipient;
        }

    }
}
