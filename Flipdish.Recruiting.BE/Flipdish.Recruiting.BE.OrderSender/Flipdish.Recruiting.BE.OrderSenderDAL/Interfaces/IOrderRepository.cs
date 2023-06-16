using Flipdish.Recruiting.BE.OrderSenderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.BE.OrderSenderDAL.Interfaces
{
    public interface IOrderRepository 
    {
        Order GetOrderById(int orderId);
    }
}
