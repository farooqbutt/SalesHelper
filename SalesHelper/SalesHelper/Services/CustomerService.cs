using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services.EncryptionServices;
using static SalesHelper.Services.ICustomerService;

namespace SalesHelper.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionDecryptionService _encdecService;

        public CustomerService(
            ApplicationDbContext context,
            EncryptionDecryptionService encdecService
            )
        {
            _context = context;
            _encdecService = encdecService;
        }

        public void Create(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Customers.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Customer Read(int id)
        {
            try
            {
                return _context.Customers.Where(c => c.Id == id)
                    .Include(c => c.AddressIdFK)
                    .Include(a => a.AccountNumberFK)
                    .FirstOrDefault()!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Customer> ReadAll()
        {
            try
            {
                return _context.Customers.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public NameType DetermineNameType(string name)
        {
            try
            {
                string[] nameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length == 1)
                {
                    return NameType.First;
                }
                else if (nameParts.Length > 1)
                {
                    return NameType.Full;
                }
                else
                {
                    return NameType.Invalid;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<CustomerEmailInterface>> ReadCustomerEmails(string CellPhone, ApplicationUser user)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync("mail.saleshelper.com", 143, false);
                await _imapClient.AuthenticateAsync(_context.Account.Find(user.AccountNumber)!.SaleshelperMailingAddress,
                                                    _encdecService.Decrypt(_context.Account.Find(user.AccountNumber)!.EncryptedSaleshelperEmailPassword!));

                List<CustomerEmailInterface> inboxMessages = await ReadInboxEmails(_imapClient, CellPhone);
                List<CustomerEmailInterface> sentMessages = await ReadSentEmails(_imapClient, CellPhone);
                List<CustomerEmailInterface> allMessages = new();
                allMessages.AddRange(inboxMessages);
                allMessages.AddRange(sentMessages);
                // sort the messages by date to show the latest email first
                allMessages.Sort((a, b) => b.Date.CompareTo(a.Date));
                _imapClient.Disconnect(true);
                return allMessages;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<List<CustomerEmailInterface>> ReadInboxEmails(ImapClient _imapClient, string CellPhone)
        {
            var inboxFolder = await _imapClient.GetFolderAsync("INBOX");
            // open the inbox folder
            await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
            // get emails from inbox folder with subject containing the PO number
            var messages = await inboxFolder.SearchAsync(SearchQuery.SubjectContains(CellPhone.ToString()));
            List<CustomerEmailInterface> emailMessages = new();
            foreach (var message in messages)
            {
                var mimeMessage = await inboxFolder.GetMessageAsync(message);
                var CustomerEmailMessage = new CustomerEmailInterface
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                SetupEmailAttachments(mimeMessage, CustomerEmailMessage);
                emailMessages.Add(CustomerEmailMessage);
            }
            return emailMessages;
        }

        private async Task<List<CustomerEmailInterface>> ReadSentEmails(ImapClient _imapClient, string CellPhone)
        {
            var sentFolder = await _imapClient.GetFolderAsync("Sent");
            // open the sent folder
            await sentFolder.OpenAsync(FolderAccess.ReadOnly);
            // get emails from sent folder with subject containing the Customer Cell Phone Number
            var messages = await sentFolder.SearchAsync(SearchQuery.SubjectContains(CellPhone.ToString()));
            List<CustomerEmailInterface> emailMessages = new();
            foreach (var message in messages)
            {
                var mimeMessage = await sentFolder.GetMessageAsync(message);
                var CustomerEmailMessage = new CustomerEmailInterface
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                SetupEmailAttachments(mimeMessage, CustomerEmailMessage);
                emailMessages.Add(CustomerEmailMessage);
            }
            return emailMessages;
        }

        private void SetupEmailAttachments(MimeMessage mimeMessage, CustomerEmailInterface CustomeremailMessage)
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
                    CustomeremailMessage.Attachments.Add(new CustomerEmailAttachmentModel
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
                    CustomeremailMessage.Attachments.Add(new CustomerEmailAttachmentModel
                    {
                        ContentType = part.ContentType.MimeType,
                        FileName = fileName,
                        Base64Data = base64
                    });
                }
            }
        }
    }
}
