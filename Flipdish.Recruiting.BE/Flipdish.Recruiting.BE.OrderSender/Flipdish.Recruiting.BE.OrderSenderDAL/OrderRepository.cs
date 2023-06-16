using Flipdish.Recruiting.BE.OrderSenderDAL.Interfaces;
using Flipdish.Recruiting.BE.OrderSenderDAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.BE.OrderSenderDAL
{
    public class OrderRepository : Interfaces.IOrderRepository
    {
        public Order GetOrderById(int orderId)
        {
            using (StreamReader sr = new StreamReader("orders.json"))
            {
                string orderJson = sr.ReadToEnd();
                IEnumerable<Order> orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(orderJson);
                return orders.Single(o => o.OrderId == orderId);
            }          
        }
    }
}
