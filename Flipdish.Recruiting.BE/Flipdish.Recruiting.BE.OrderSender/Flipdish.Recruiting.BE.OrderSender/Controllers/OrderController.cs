using System;
using Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces;
using Flipdish.Recruiting.BE.OrderSenderDAL;
using Flipdish.Recruiting.BE.OrderSenderDAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Flipdish.Recruiting.BE.OrderSender.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private IOrderRepository _orderRepository;
        private readonly ILogger<OrderController> _logger;
        private IMailHelper _mailHelper;

        public OrderController(ILogger<OrderController> logger, IMailHelper mailHelper)
        {
            _orderRepository = new OrderRepository();
            _mailHelper = mailHelper;
            _logger = logger;
        }
        [Authorize]
        [HttpGet]
        public IActionResult SendOrder(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            var mailTemplate = _mailHelper.CreateMailTemplate(order);
            try
            {
                _mailHelper.SendEmail("interview@flipdish.com", new[] { "to@flipdish.com" }, mailTemplate);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error sending email for order: {orderId}";
                _logger.LogError(e, errorMessage);
                return BadRequest(errorMessage);
            }

            return Ok("Order Sent!");
        }
    }
}