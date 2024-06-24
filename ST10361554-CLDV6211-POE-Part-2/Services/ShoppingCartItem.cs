using ST10361554_CLDV6211_POE_Part_2.Models;

namespace ST10361554_CLDV6211_POE_Part_2.Services
{
    public class ShoppingCartItem
    {
        public int CraftworkId { get; set; }
        public int Quantity { get; set; }

        public required Craftwork craftwork { get; set; }
    }
}
