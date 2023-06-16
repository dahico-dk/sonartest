using Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces;
using Flipdish.Recruiting.BE.OrderSenderDAL.Models;

namespace Flipdish.Recruiting.BE.OrderSender.Utilities
{
    public class OrderHelper : IOrderHelper
    {
        public const decimal taxPercentage = 0.21m;
        public decimal CalculateTax(Order order)
        {
            return (order.FoodAmount + order.TipAmount) * taxPercentage;
        }

    }
}
