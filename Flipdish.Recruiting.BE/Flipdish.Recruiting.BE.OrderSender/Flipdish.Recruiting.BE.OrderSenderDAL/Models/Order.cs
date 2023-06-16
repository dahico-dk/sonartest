namespace Flipdish.Recruiting.BE.OrderSenderDAL.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public decimal FoodAmount { get; set; }
        public decimal TipAmount { get; set; }
    }
}
