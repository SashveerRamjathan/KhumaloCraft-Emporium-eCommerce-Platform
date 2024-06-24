using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public decimal OrderTotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual AspNetUser User { get; set; } = null!;
}
