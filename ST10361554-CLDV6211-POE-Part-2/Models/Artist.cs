using System;
using System.Collections.Generic;

namespace ST10361554_CLDV6211_POE_Part_2.Models;

public partial class Artist
{
    public string ArtistId { get; set; } = null!;

    public string? ArtistName { get; set; }

    public string? ArtistEmail { get; set; }

    public string? ArtistDescription { get; set; }

    public byte[]? ArtistPictureData { get; set; }

    public string? ArtistPictureUrl { get; set; }

    public virtual AspNetUser ArtistNavigation { get; set; } = null!;

    public virtual ICollection<Craftwork> Craftworks { get; set; } = new List<Craftwork>();
}
