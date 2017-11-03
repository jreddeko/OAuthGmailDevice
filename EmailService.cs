using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Wddc.Email
{
    public class EmailService
    {
        public static async Task SendMessageAsync(SmtpClient client, MailMessage message)
        {
            await client.SendMailAsync(message);
        }

        public static void SendMessage(string from, string[] recipients, string subject, string body, string host
            , int port, string username, string password)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(from);
                foreach (var f in recipients)
                {
                    mail.To.Add(f);
                }
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(username, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        public static MailMessage GetMessage(string from, string to, string subject, string body)
        {
            return new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
            };
        }

        public static SmtpClient GetSmtpClient(int port, SmtpDeliveryMethod method, string host, bool useDefaultCredentials)
        {
            return new SmtpClient()
            {
                Port = port,
                DeliveryMethod = method,
                UseDefaultCredentials = useDefaultCredentials,
                Host = host
            };
        }
    }
}
