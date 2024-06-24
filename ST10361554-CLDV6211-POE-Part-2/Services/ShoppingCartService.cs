namespace ST10361554_CLDV6211_POE_Part_2.Services
{
    public class ShoppingCartService
    {
        public List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();

        public void AddToCart(ShoppingCartItem cartItem)
        {
            // Check if the item already exists in the cart
            var existingItem = cartItems.FirstOrDefault(item => item.CraftworkId == cartItem.CraftworkId);

            if (existingItem != null)
            {
                // Update quantity if the item is already in the cart
                existingItem.Quantity += cartItem.Quantity;
            }
            else
            {
                // Add new item to the cart
                cartItems.Add(cartItem);
            }
        }

        public void RemoveFromCart(int craftworkId)
        {
            var itemToRemove = cartItems.FirstOrDefault(item => item.CraftworkId == craftworkId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }
        }

        public void ClearCart()
        {
            cartItems.Clear();
        }

        public List<ShoppingCartItem> GetCartItems()
        {
            return cartItems;
        }
    }
}
