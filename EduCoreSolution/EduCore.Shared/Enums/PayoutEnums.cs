using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.Enums
{
    // Lifecycle of a single earning record (one per paid enrollment)
    public enum EarningStatus
    {
        Pending,    // Created but not yet confirmed (e.g., payment still processing)
        Available,  // Confirmed and ready to be invoiced
        Invoiced,   // Added to a monthly invoice
        Paid,       // Invoice was paid out to teacher
        Cancelled   // Refunded or voided
    }

    // Lifecycle of a monthly invoice
    public enum InvoiceStatus
    {
        Draft,      // Created but not yet issued to the teacher
        Issued,     // Visible to teacher, awaiting payment
        Paid,       // Admin paid the teacher
        Cancelled   // Voided
    }

    // How the teacher is paid out
    public enum PayoutMethod
    {
        Cash,
        BankTransfer,
        VodafoneCash,
        InstaPay,
        Other
    }
}
