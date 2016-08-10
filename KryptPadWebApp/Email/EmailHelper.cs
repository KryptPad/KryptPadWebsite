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
            // Credentials
            var credentialUserName = ConfigurationManager.AppSettings["SmtpUserName"];
            var sentFrom = ConfigurationManager.AppSettings["SmtpSendFrom"];
            var pwd = ConfigurationManager.AppSettings["SmtpPassword"];
            var server = ConfigurationManager.AppSettings["SmtpHostName"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

            // Configure the client
            var client = new SmtpClient(server);

            client.Port = port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials
            var credentials = new NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message
            var mail = new MailMessage(sentFrom, to);
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;

            // Send
            return client.SendMailAsync(mail);
        }
    }
}
