using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.EmailModels;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services.EmailService
{
    public class EmailService : IEmailService
    {
        //private static readonly string client_id = "35558157-23e0-4189-988d-b665bc6811c5";
        //private static readonly string tenant_id = "867180a3-f90d-4e3b-919e-15446003424f";
        //private static readonly string client_secret = "H~f8Q~hLeJ5E7s_nZsl9EsIOOQrIeMhe2RUudbIw";

        private readonly ApplicationDbContext _context;
        private readonly MailSettings _mailSettings;
        //private readonly ImapClient _imapClient;


        public EmailService(IOptions<MailSettings> mailSettings, ApplicationDbContext context)
        {
            _context = context;
            _mailSettings = mailSettings.Value;

            //// imap client to connect to the email server
            //var options = new PublicClientApplicationOptions
            //{
            //    ClientId = client_id,
            //    TenantId = tenant_id,

            //    // Use "https://login.microsoftonline.com/common/oauth2/nativeclient" for apps using
            //    // embedded browsers or "http://localhost" for apps that use system browsers.
            //    RedirectUri = "http://localhost:5074"
            //};

            //var publicClientApplication = PublicClientApplicationBuilder
            //    .CreateWithApplicationOptions(options)
            //    .Build();

            //var scopes = new string[]
            //{
            //    "https://outlook.office.com/IMAP.AccessAsUser.All", // Only needed for IMAP
            //};

            //var authToken = publicClientApplication.AcquireTokenByUsernamePassword(scopes, _mailSettings.Email, _mailSettings.Password).ExecuteAsync();
            //var oauth2 = new SaslMechanismOAuth2(authToken.Result.Account.Username, authToken.Result.AccessToken);

            //_imapClient = new ImapClient();
            //_imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
            //_imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
        }

        //public void Dispose()
        //{
        //    _imapClient.Disconnect(true);
        //    _imapClient.Dispose();
        //}

        public Task SendSimpleEmail(string to, string subject, string body, IFormFile attachment, QuotationEmails quoteEmail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;

                if (attachment != null)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.OpenReadStream());
                }

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.SMTP_Server, _mailSettings.SMTP_Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                smtp.Send(email);
                smtp.Disconnect(true);

                // creating a record of the email sent in our database
                quoteEmail.EmailMessageId = email.MessageId;
                CreateQuoteEmail(quoteEmail);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task SendEstimateRequestEmail(string to, string subject, string body, string attachmentName, byte[]? pdfBytes, IFormFile attachment, QuotationEmails quoteEmail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = body;

                // if attachment is not null, add it to the email using mailkit attachment
                if (pdfBytes != null)
                {
                    bodyBuilder.Attachments.Add(attachmentName, pdfBytes);
                }

                if (attachment != null)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.OpenReadStream());
                }

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.SMTP_Server, _mailSettings.SMTP_Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                smtp.Send(email);
                smtp.Disconnect(true);

                // creating a record of the email sent in our database
                quoteEmail.EmailMessageId = email.MessageId;
                CreateQuoteEmail(quoteEmail);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // create a record of the email sent in our database
        private void CreateQuoteEmail(QuotationEmails quoteEmail)
        {
            try
            {
                _context.QuotationEmails.Add(quoteEmail);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InboxUnReadCount(List<string> searchFromEmailIds, ImapClient imapClient)
        {
            int count = 0;
            var inboxFolder = await imapClient.GetFolderAsync("INBOX");
            // open the inbox folder
            await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
            foreach (var id in searchFromEmailIds)
            {
                // search for emails with the same message id in the references or in-reply-to headers with unread status
                var messages = await inboxFolder.SearchAsync(SearchQuery.And(
                                       SearchQuery.HeaderContains("References", id),
                                                          SearchQuery.NotSeen
                                                                         ));
                count += messages.Count;
            }
            return count;
        }

        private void SetupEmailAttachments(MimeMessage mimeMessage, EmailMessage emailMessage)
        {
            foreach (var attachment in mimeMessage.Attachments)
            {
                if (attachment is MessagePart)
                {
                    var fileName = attachment.ContentDisposition?.FileName;
                    var rfc822 = (MessagePart)attachment;

                    if (string.IsNullOrEmpty(fileName))
                        fileName = "attached-message.eml";

                    using var stream = new MemoryStream();
                    rfc822.Message.WriteTo(stream);
                    var fileBytes = stream.ToArray();
                    var base64 = Convert.ToBase64String(fileBytes);
                    emailMessage.Attachments.Add(new AttachmentModel
                    {
                        ContentType = rfc822.ContentType.MimeType,
                        FileName = fileName,
                        Base64Data = base64
                    });
                }
                else
                {
                    var part = (MimePart)attachment;
                    var fileName = part.FileName;

                    using var stream = new MemoryStream();
                    part.Content.DecodeTo(stream);
                    var fileBytes = stream.ToArray();
                    var base64 = Convert.ToBase64String(fileBytes);
                    emailMessage.Attachments.Add(new AttachmentModel
                    {
                        ContentType = part.ContentType.MimeType,
                        FileName = fileName,
                        Base64Data = base64
                    });
                }
            }
        }

        public async Task<QuoteEmailsInterface> GetQuoteInboxEmails(int quoteId)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
                await _imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                // TODO: to be removed after testing
                // get folders list first for debugging purposes
                //var folders = await _imapClient.GetFoldersAsync(new FolderNamespace('.', ""));
                /////////////////////////////////
                var inboxFolder = await _imapClient.GetFolderAsync("INBOX");
                // open the inbox folder
                await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
                // get email ids from our database for the quote
                var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId).Select(qe => qe.EmailMessageId).ToList();
                if (emailIds.Count > 0)
                {
                    var quoteEmailInterface = new QuoteEmailsInterface();
                    var emailMessages = new List<EmailMessage>();
                    foreach (var id in emailIds)
                    {
                        var messages = await inboxFolder.SearchAsync(SearchQuery.Or(
                            SearchQuery.HeaderContains("References", id),
                            SearchQuery.HeaderContains("In-Reply-To", id)
                        ));

                        foreach (var message in messages)
                        {
                            var mimeMessage = await inboxFolder.GetMessageAsync(message);
                            // Check if the message is starred
                            var summary = await inboxFolder.FetchAsync(new[] { message }, MessageSummaryItems.Flags | MessageSummaryItems.GMailLabels);

                            var email = new EmailMessage
                            {
                                MessageId = mimeMessage.MessageId,
                                From = mimeMessage.From.ToString(),
                                To = mimeMessage.To.ToString(),
                                Subject = mimeMessage.Subject,
                                Body = mimeMessage.HtmlBody,
                                Date = mimeMessage.Date.DateTime,
                                IsRead = summary[0].Flags!.Value.HasFlag(MessageFlags.Seen),
                                IsStarred = summary[0].Flags!.Value.HasFlag(MessageFlags.Flagged),
                                HasAttachments = mimeMessage.Attachments.Count() > 0
                            };
                            // add the quoteEmail to the list
                            emailMessages.Add(email);
                        }
                    }

                    quoteEmailInterface.QuoteId = quoteId;
                    quoteEmailInterface.QuoteType = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId).First().QuoteType;
                    quoteEmailInterface.InboxUnReadCount = await InboxUnReadCount(emailIds, _imapClient);
                    quoteEmailInterface.FolderName = "Inbox";
                    quoteEmailInterface.Emails = emailMessages;

                    _imapClient.Disconnect(true);
                    return quoteEmailInterface;
                }
                else
                {
                    _imapClient.Disconnect(true);
                    return new QuoteEmailsInterface();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<QuoteEmailsInterface> GetQuoteSentEmails(int quoteId)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
                await _imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                var sentFolder = await _imapClient.GetFolderAsync("[Gmail]/Sent Mail");
                // open the inbox folder
                await sentFolder.OpenAsync(FolderAccess.ReadOnly);
                // get email ids from our database for the quote
                var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId).Select(qe => qe.EmailMessageId).ToList();
                if (emailIds.Count > 0)
                {
                    var quoteEmailInterface = new QuoteEmailsInterface();
                    var emailMessages = new List<EmailMessage>();
                    foreach (var id in emailIds)
                    {
                        var messages = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", id));

                        foreach (var message in messages)
                        {
                            var mimeMessage = await sentFolder.GetMessageAsync(message);
                            // Check if the message is starred
                            var summary = await sentFolder.FetchAsync(new[] { message }, MessageSummaryItems.Flags | MessageSummaryItems.GMailLabels);

                            var email = new EmailMessage
                            {
                                MessageId = mimeMessage.MessageId,
                                From = mimeMessage.From.ToString(),
                                To = mimeMessage.To.ToString(),
                                Subject = mimeMessage.Subject,
                                Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                                Date = mimeMessage.Date.DateTime,
                                IsStarred = summary[0].Flags!.Value.HasFlag(MessageFlags.Flagged),
                                HasAttachments = mimeMessage.Attachments.Count() > 0

                        };
                            // add the quoteEmail to the list
                            emailMessages.Add(email);
                        }
                    }

                    quoteEmailInterface.QuoteId = quoteId;
                    quoteEmailInterface.QuoteType = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId).First().QuoteType;
                    quoteEmailInterface.InboxUnReadCount = await InboxUnReadCount(emailIds, _imapClient);
                    quoteEmailInterface.FolderName = "Sent";
                    quoteEmailInterface.Emails = emailMessages;

                    _imapClient.Disconnect(true);
                    return quoteEmailInterface;
                }
                else
                {
                    _imapClient.Disconnect(true);
                    return new QuoteEmailsInterface();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmailMessage> GetInboxEmailMessageDetails(string messageId)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
                await _imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                var inboxFolder = await _imapClient.GetFolderAsync("INBOX");
                await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
                var messages = await inboxFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", messageId));
                var mimeMessage = await inboxFolder.GetMessageAsync(messages[0]);
                // create the email message object
                var emailMessage = new EmailMessage
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                // get the attachments
                SetupEmailAttachments(mimeMessage, emailMessage);
                _imapClient.Disconnect(true);
                return emailMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmailMessage> GetSentEmailMessageDetails(string messageId)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
                await _imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                var sentFolder = await _imapClient.GetFolderAsync("[Gmail]/Sent Mail");
                await sentFolder.OpenAsync(FolderAccess.ReadOnly);
                var messages = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", messageId));
                var mimeMessage = await sentFolder.GetMessageAsync(messages[0]);
                var emailMessage = new EmailMessage
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                // get the attachments
                SetupEmailAttachments(mimeMessage, emailMessage);
                _imapClient.Disconnect(true);
                return emailMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
