using ST10361554_CLDV6211_POE_Part_2.Services;

namespace ST10361554_CLDV6211_POE_Part_2.View_Models
{
    public class ShoppingCartViewModel
    {
        public required List<ShoppingCartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
