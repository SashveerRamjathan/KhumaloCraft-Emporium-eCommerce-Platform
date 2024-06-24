using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace ST10361554_CLDV6211_Order_Workflow_Function.Models;

public partial class Craftwork
{
    public int CraftworkId { get; set; }

    public string CraftworkName { get; set; } = null!;

    public string CraftworkDescription { get; set; }

    public decimal CraftworkPrice { get; set; }

    public int CraftworkQuantity { get; set; }

    public int? CategoryId { get; set; }

    public string ArtistId { get; set; } = null!;

    public byte[] CraftworkPictureData { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual CraftworkCategory Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
}
