using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Services.EncryptionServices;

namespace SalesHelper.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly EncryptionDecryptionService _encdecService;
        public EmailService(ApplicationDbContext context, IConfiguration config, EncryptionDecryptionService encdecService)
        {
            _context = context;
            _config = config;
            _encdecService = encdecService;
        }

        private Account EmailSender(int accountNumber)
        {
            return _context.Account.Find(accountNumber)!;
        }

        public async Task<bool> SendAccountConfirmationEmail(string to, string subject, string htmlMessage)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetValue<string>("SaleshelperTeamAccount:Email")));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.GetValue<string>("SaleshelperTeamAccount:Server"), _config.GetValue<int>("SaleshelperTeamAccount:SMTP_Port"), false);
                await smtp.AuthenticateAsync(_config.GetValue<string>("SaleshelperTeamAccount:Email"), _config.GetValue<string>("SaleshelperTeamAccount:Password"));
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                SaveMessageCopyToSentFolder(email); // save a copy of the email in the sent folder

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task SendEstimateRequestEmail(int accountNumber, string to, string subject, string body, string attachmentName, byte[]? pdfBytes, IFormFile attachment, QuotationEmails quoteEmail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(EmailSender(accountNumber).SaleshelperMailingAddress));
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
                smtp.Connect(_config.GetValue<string>("HMailServerSettings:Server"), _config.GetValue<int>("HMailServerSettings:SMTP_Port"), false);
                smtp.Authenticate(EmailSender(accountNumber).SaleshelperMailingAddress, _encdecService.Decrypt(EmailSender(accountNumber).EncryptedSaleshelperEmailPassword!));
                smtp.Send(email);
                smtp.Disconnect(true);

                SaveMessageCopyToSentFolderForUsers(email, EmailSender(accountNumber).SaleshelperMailingAddress!, _encdecService.Decrypt(EmailSender(accountNumber).EncryptedSaleshelperEmailPassword!)); // save a copy of the email in the sent folder

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

        //public async Task<EmailMessage> GetSentEmailMessageDetails(string messageId)
        //{
        //    try
        //    {
        //        using var _imapClient = new ImapClient();
        //        await _imapClient.ConnectAsync(_mailSettings.IMAP_Server, _mailSettings.IMAP_Port, SecureSocketOptions.SslOnConnect);
        //        await _imapClient.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
        //        var sentFolder = await _imapClient.GetFolderAsync("[Gmail]/Sent Mail");
        //        await sentFolder.OpenAsync(FolderAccess.ReadOnly);
        //        var messages = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", messageId));
        //        var mimeMessage = await sentFolder.GetMessageAsync(messages[0]);
        //        var emailMessage = new EmailMessage
        //        {
        //            MessageId = mimeMessage.MessageId,
        //            From = mimeMessage.From.ToString(),
        //            To = mimeMessage.To.ToString(),
        //            Subject = mimeMessage.Subject,
        //            Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
        //            Date = mimeMessage.Date.DateTime
        //        };
        //        // get the attachments
        //        SetupEmailAttachments(mimeMessage, emailMessage);
        //        _imapClient.Disconnect(true);
        //        return emailMessage;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        private async void SaveMessageCopyToSentFolder(MimeMessage? email)
        {
            try
            {
                using var imap = new ImapClient();
                await imap.ConnectAsync(_config.GetValue<string>("SaleshelperTeamAccount:Server"), _config.GetValue<int>("SaleshelperTeamAccount:IMAP_Port"), false);
                await imap.AuthenticateAsync(_config.GetValue<string>("SaleshelperTeamAccount:Email"), _config.GetValue<string>("SaleshelperTeamAccount:Password"));

                var sentFolder = imap.GetFolder("Sent");
                await sentFolder.OpenAsync(FolderAccess.ReadWrite);
                await sentFolder.AppendAsync(email);
                var message = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", email!.MessageId));
                await sentFolder.SetFlagsAsync(message.First(), MessageFlags.Seen, true);
                await imap.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async void SaveMessageCopyToSentFolderForUsers(MimeMessage? email, string saleshelperMailingAddress, string password)
        {
            try
            {
                using var imap = new ImapClient();
                await imap.ConnectAsync(_config.GetValue<string>("HMailServerSettings:Server"), _config.GetValue<int>("HMailServerSettings:IMAP_Port"), false);
                await imap.AuthenticateAsync(saleshelperMailingAddress, password);

                var sentFolder = imap.GetFolder("Sent");
                await sentFolder.OpenAsync(FolderAccess.ReadWrite);
                await sentFolder.AppendAsync(email);
                var message = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", email!.MessageId));
                await sentFolder.SetFlagsAsync(message.First(), MessageFlags.Seen, true);
                await imap.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
