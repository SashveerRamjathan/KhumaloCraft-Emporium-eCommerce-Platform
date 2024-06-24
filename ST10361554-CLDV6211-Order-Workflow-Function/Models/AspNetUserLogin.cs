using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class AspNetUserLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string ProviderDisplayName { get; set; }

    public string UserId { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
