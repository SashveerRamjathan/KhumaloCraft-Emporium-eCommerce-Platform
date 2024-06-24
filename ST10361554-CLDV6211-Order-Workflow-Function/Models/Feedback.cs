using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class Feedback
{
    public string UserId { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
