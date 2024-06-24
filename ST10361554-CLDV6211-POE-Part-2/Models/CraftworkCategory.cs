using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_POE_Part_2.Models;

public partial class CraftworkCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Craftwork> Craftworks { get; set; } = new List<Craftwork>();
}
