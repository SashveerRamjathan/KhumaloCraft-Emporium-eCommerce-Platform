using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class AspNetUserToken
{
    public string UserId { get; set; } = null!;

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Value { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
