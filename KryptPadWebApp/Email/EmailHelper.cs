using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace KryptPadWebApp.Email
{
    public class EmailHelper
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Task SendAsync(string subject, string body, string to)
        {
            // Configure the client
            var client = new SmtpClient();

            // Create the message
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(to));
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;

            // Send
            return client.SendMailAsync(mail);
        }
    }
}
