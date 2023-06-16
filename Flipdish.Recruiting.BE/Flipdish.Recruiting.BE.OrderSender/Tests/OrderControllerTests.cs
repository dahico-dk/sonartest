using Flipdish.Recruiting.BE.OrderSender.Controllers;
using Flipdish.Recruiting.BE.OrderSender.Utilities;
using Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces;
using Flipdish.Recruiting.BE.OrderSenderDAL;
using Flipdish.Recruiting.BE.OrderSenderDAL.Interfaces;
using Flipdish.Recruiting.BE.OrderSenderDAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace Tests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private IOrderHelper _orderHelper;
        private IOrderRepository _orderRepository;
        private Order mockOrder;
        private Mock<ILogger<OrderController>> _mockLogger;
        private Mock<IMailHelper> _mockMailHelper;


        [OneTimeSetUp]
        public void Setup()
        {
            Order mockOrder = new Order
            {
                OrderId = 5,
                FoodAmount = 42,
                RestaurantName = "Bob's Burgers",
                TipAmount = 42
            };

            var orderHelperMock = new Mock<IOrderHelper>();
            var orderRepositoryMock = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderController>>();
            _mockMailHelper = new Mock<IMailHelper>();
            // Set up the mock repository to return some data
            orderRepositoryMock.Setup(repo => repo.GetOrderById(It.IsAny<int>()))
                .Returns(mockOrder);
            // Set up the order helper to return some data
            orderHelperMock.Setup(helper => helper.CalculateTax(It.IsAny<Order>())).Returns(21m);

            _orderRepository = orderRepositoryMock.Object;
            _orderHelper = orderHelperMock.Object;

        }


        [Test]
        public void CalculateTax_CalculatesAt21Percent()
        {

            //Act
            decimal tax = _orderHelper.CalculateTax(mockOrder);

            //Assert
            Assert.AreEqual(21m, tax);
        }

        [Test]
        public void LoadOrder_LoadsGivenOrder()
        {
            //Arrange
            int orderId = 5;

            //Act
            var order = _orderRepository.GetOrderById(orderId);

            //Assert
            Assert.AreEqual(5, order.OrderId);
            Assert.AreEqual("Bob's Burgers", order.RestaurantName);
            Assert.AreEqual(42, order.FoodAmount);
            Assert.AreEqual(42, order.TipAmount);
        }

        [Test]
        public void SendOrder_Returns_Ok_When_Email_Is_Sent_Successfully()
        {

            // Arrange
            _mockMailHelper.Reset();
            var _orderController = new OrderController(_mockLogger.Object, _mockMailHelper.Object);

            //Act
            var result = _orderController.SendOrder(1);

            // Assert           
            Assert.AreEqual("Order Sent!", ((OkObjectResult)result).Value);
        }

        [Test]
        public void SendOrder_Returns_BadRequest_When_Email_Fails_To_Send()
        {

            // Arrange             
            _mockMailHelper.Setup(a => a.SendEmail(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<MailTemplate>(), null, null)).Throws<Exception>();
            var _orderController = new OrderController(_mockLogger.Object, _mockMailHelper.Object);

            //Act
            var result = _orderController.SendOrder(1);

            // Assert
            Assert.AreEqual("Error sending email for order: 1", ((BadRequestObjectResult)result).Value);
        }


        [Test]
        public void SendEmail_ShouldSend()
        {
            //Arrange
            string from = "from.test@flipdish.com";
            List<string> to = new List<string>() { "to.test@flipdish.com" };
            MailTemplate template = new MailTemplate()
            {
                Body = "Your order has been placed with the restaurant.",
                Subject = "My Test Email"
            };
            _mockMailHelper.Reset();
            _mockMailHelper.Setup(a => a.SendEmail(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<MailTemplate>(), null, null));
            var mailhelper = _mockMailHelper.Object;
            //Act
            mailhelper.SendEmail(from, to, template, null, null);         
        }

        [Test]
        public void SendEmail_ShouldThrowSmtpException()
        {
            //Arrange
            string from = "from.test@flipdish.com";
            List<string> to = new List<string>() { "to.test@flipdish.com" };
            MailTemplate template = new MailTemplate()
            {
                Body = "Your order has been placed with the restaurant.",
                Subject = "My Test Email"
            };
            _mockMailHelper.Reset();
            _mockMailHelper.Setup(a => a.SendEmail(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<MailTemplate>(), null, null)).Throws(new SmtpException());
            var mailhelper = _mockMailHelper.Object;

            //Act and Assert           
            Assert.Throws<SmtpException>(() => mailhelper.SendEmail(from, to, template, null, null));
        }
    }
}
