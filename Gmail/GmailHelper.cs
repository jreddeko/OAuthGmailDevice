using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using System;
using System.IO;
using System.Threading;


namespace Wddc.Email.Gmail
{
    public class GmailHelper
    {
        private string clientSecretPath;
        private string appName;
        private string[] scopes;

        public GmailHelper(string appName, string clientSecretPath)
        {
            this.appName = appName;
            this.clientSecretPath = clientSecretPath;
            this.scopes = new string[] { GmailService.Scope.MailGoogleCom };
        }

        public void SendEmail(string fromId, string subject, string body, string toIds, string[] attachments)
        {
            var userCredential = Authorize(fromId);

            // Create Gmail API service.
            var service = new GmailServiceExtensions(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
                ApplicationName = this.appName,
            });
            // send message
            MimeMessage message = GetMimeMessage(fromId, fromId, toIds, subject, body, attachments);
            service.SendMessage(userCredential.UserId, message);
        }

        /// <summary>
        /// Authorizes the user using Oauth2
        /// </summary>
        /// <param name="clientSecretFile">json file containing client id and secret</param>
        /// <param name="username"></param>
        /// <param name="scopes">https://developers.google.com/identity/protocols/googlescopes</param>
        /// <returns></returns>
        public UserCredential Authorize(string username)
        {
            using (var stream =
                new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, String.Format(".credentials/{0}.json", username));

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    username,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        private MimeMessage GetMimeMessage(string fromId, string fromName, string toIds, string subject, string body, string[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromId));
            message.To.Add(new MailboxAddress(toIds, toIds));
            message.Subject = subject;

            var textPartBody = new TextPart("plain")
            {
                Text = body
            };

            var multipart = new Multipart("mixed");
            multipart.Add(textPartBody);
            foreach (var att in attachments)
            {
                var extension = Path.GetExtension(att);
                var attachment = new MimePart(MimeTypes.MimeTypeMap.GetMimeType(extension), extension)
                {
                    ContentObject = new ContentObject(File.OpenRead(att), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(att)
                };
                multipart.Add(attachment);
            }
            message.Body = multipart;
            return message;
        }
    }
}
