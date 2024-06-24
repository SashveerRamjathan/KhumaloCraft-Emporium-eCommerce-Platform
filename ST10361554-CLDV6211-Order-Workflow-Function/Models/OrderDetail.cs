using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int CraftworkId { get; set; }

    public int Quantity { get; set; }

    public string CraftworkStatus { get; set; } = null!;

    public virtual Craftwork Craftwork { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
