using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.EnrollmentDTOs
{
    public class PaymobWebhookDto
    {
        public bool success { get; set; }
        public string? hmac { get; set; }
        public PaymobTransactionDto? obj { get; set; }
    }

    public class PaymobTransactionDto
    {
        public int id { get; set; }
        public bool success { get; set; }
        public int amount_cents { get; set; }
        public string? currency { get; set; }
        public DateTime created_at { get; set; }
        public bool error_occured { get; set; }
        public bool has_parent_transaction { get; set; }
        public int integration_id { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public bool is_refunded { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public PaymobOrderDto? order { get; set; }
        public int owner { get; set; }
        public bool pending { get; set; }
        public PaymobSourceDataDto? source_data { get; set; }
    }

    public class PaymobOrderDto
    {
        public int id { get; set; }
        public string? merchant_order_id { get; set; }
    }

    public class PaymobSourceDataDto
    {
        public string? pan { get; set; }
        public string? sub_type { get; set; }
        public string? type { get; set; }
    }
}
