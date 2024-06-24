using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10361554_CLDV6211_Order_Notification_Email_Sender
{
    public class OrderEventData
    {
        public string OrderId { get; set; }
        public string UserEmail { get; set; }
        public string Status { get; set; }
        public string NotificationDate { get; set; }
    }
}
