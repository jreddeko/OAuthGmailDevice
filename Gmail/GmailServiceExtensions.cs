using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using System;
using System.IO;

namespace Wddc.Email.Gmail
{
    internal class GmailServiceExtensions : GmailService
    {
        public GmailServiceExtensions(Initializer initializer) : base(initializer)
        {
        }

        public Message SendMessage(string userId, MimeMessage emailContent)
        {
            try
            {
                Message message = CreateMessageWithEmail(emailContent);
                message = this.Users.Messages.Send(message, userId).Execute();
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error sending message. From:'{0}' To:'{1}'. Subject:'{2}'. Body:'{3}'"
                    , userId, emailContent.To, emailContent.Subject, emailContent.Body), ex);
            }
        }

        public static Message CreateMessageWithEmail(MimeMessage emailContent)
        {
            using (var stream = new MemoryStream())
            {
                emailContent.WriteTo(stream);
                Message message = new Message();
                message.Raw = Base64Encode(emailContent);
                return message;
            }

        }
        public static string Base64Encode(MimeMessage message)
        {
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);

                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");
            }
        }
    }
}