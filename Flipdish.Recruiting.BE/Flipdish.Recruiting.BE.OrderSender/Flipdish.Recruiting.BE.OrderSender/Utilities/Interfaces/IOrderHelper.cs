using Flipdish.Recruiting.BE.OrderSenderDAL.Models;

namespace Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces
{
    public interface IOrderHelper
    {
        decimal CalculateTax(Order order);
    }
}
