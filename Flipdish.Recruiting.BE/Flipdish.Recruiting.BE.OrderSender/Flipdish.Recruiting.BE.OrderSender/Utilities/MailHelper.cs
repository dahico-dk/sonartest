using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using Flipdish.Recruiting.BE.OrderSenderDAL.Models;
using Microsoft.Extensions.Configuration;
using Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces;
using System;

namespace Flipdish.Recruiting.BE.OrderSender.Utilities
{

    public class MailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MailOptions
    {
        public const string MailSettings = "MailSettings";

        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }


    public class MailHelper : IMailHelper
    {
        private IOrderHelper _orderHelper;
        private IConfiguration _configuration;

        public MailHelper(IOrderHelper orderHelper, IConfiguration configuration)
        {
            _orderHelper = orderHelper;
            _configuration = configuration;
        }
        public MailTemplate CreateMailTemplate(Order order)
        {
            decimal taxAmount = _orderHelper.CalculateTax(order);
            var total = order.FoodAmount + order.TipAmount + taxAmount;
            return new MailTemplate()
            {
                Subject = $"Order Received for {order.RestaurantName}",
                Body = $@"
               <p> OrderId: {order.OrderId} </p>
               <p> Restaurant: {order.RestaurantName}</p>
               <p> FoodAmount: {order.FoodAmount}</p>
               <p> TipAmount: {order.TipAmount}</p>
               <p> TaxAmount: {taxAmount}</p> <br>
               <p> Total: {total} </p>"
            };

        }

        public void SendEmail(string from, IEnumerable<string> to, MailTemplate template,
          Dictionary<string, Stream> attachements = null, IEnumerable<string> cc = null)
        {
            var mailMessage = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(from),
                Subject = template.Subject,
                Body = template.Body
            };

            if (to != null)
                mailMessage.To.Add(string.Join(",", to));
            if (cc != null)
                mailMessage.CC.Add(string.Join(",", cc));

            AddAttachments(attachements, mailMessage);
            var mailOptions = new MailOptions();
            _configuration.GetSection(MailOptions.MailSettings).Bind(mailOptions);

            using var smtpClient = new SmtpClient
            {
                Host = mailOptions.Host,
                //Port = mailOptions.Port,
                Credentials = new NetworkCredential(mailOptions.Username, mailOptions.Password),
            };

            smtpClient.SendCompleted += (sender, e) => {
                mailMessage.Dispose();
                smtpClient.Dispose();
            };
            //commented to check swagger
            //smtpClient.Send(mailMessage);
        }

        private void AddAttachments(Dictionary<string, Stream> attachements, MailMessage mailMessage)
        {
            if (attachements != null)
            {
                foreach (KeyValuePair<string, Stream> nameAndStreamPair in attachements)
                {
                    var attachment = new Attachment(nameAndStreamPair.Value, nameAndStreamPair.Key);
                    attachment.ContentId = nameAndStreamPair.Key;
                    mailMessage.Attachments.Add(attachment);
                }
            }
        }
    }
}
