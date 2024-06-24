using ST10361554_CLDV6211_Order_Workflow_Function.Models;

namespace ST10361554_CLDV6211_Order_Workflow_Function
{
    public class ShoppingCartItem
    {
        public int CraftworkId { get; set; }
        public int Quantity { get; set; }

        public required Craftwork craftwork { get; set; }
    }
}
