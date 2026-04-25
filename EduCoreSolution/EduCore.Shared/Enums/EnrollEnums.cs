using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.Enums
{

    public enum CashRequestStatus
    {
        Pending,
        Confirmed,
        Rejected
    }
    public enum PaymentMethod { CreditCard, PayPal, BankTransfer, Stripe, Fawry,Paymob }
    public enum PaymentStatus { Pending, Completed, Failed, Refunded }
}
