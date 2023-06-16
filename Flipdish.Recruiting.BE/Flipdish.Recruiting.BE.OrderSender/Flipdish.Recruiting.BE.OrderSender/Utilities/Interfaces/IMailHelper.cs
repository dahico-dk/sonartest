using Flipdish.Recruiting.BE.OrderSenderDAL.Models;
using System.Collections.Generic;
using System.IO;

namespace Flipdish.Recruiting.BE.OrderSender.Utilities.Interfaces
{
    public interface IMailHelper
    {
        MailTemplate CreateMailTemplate(Order order);
        void SendEmail(string from, IEnumerable<string> to, MailTemplate template,
          Dictionary<string, Stream> attachements = null, IEnumerable<string> cc = null);
    }
}
