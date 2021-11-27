using System.Text;

namespace RabbitMQ_Test_Service.Models
{
    class SimpleMessage
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Sender))
                sb.AppendLine($"{Sender} sent");
            if (!string.IsNullOrEmpty(Header))
                sb.AppendLine($"Message titled \"{Header}\":");
            sb.AppendLine($"{Body}");
            return sb.ToString();
        }
    }
}
