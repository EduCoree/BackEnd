using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.Settings
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public string WebhookSecret { get; set; } = string.Empty;
        public int IFrameId { get; set; }
    }
}
