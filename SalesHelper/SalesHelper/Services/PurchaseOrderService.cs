using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services.EncryptionServices;

namespace SalesHelper.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionDecryptionService _encdecService;
        public PurchaseOrderService(
            ApplicationDbContext context,
            EncryptionDecryptionService encdecService
            )
        {
            _context = context;
            _encdecService = encdecService;
        }

        public void Create(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.ModifiedDate = DateTime.Now;
                _context.PurchaseOrders.Add(purchaseOrder);
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
                _context.PurchaseOrders.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public PurchaseOrder Read(int id)
        {
            try
            {
                return _context.PurchaseOrders.Where(a => a.Id == id)
                    .Include(a => a.SalesOrderFK)
                    .Include(a => a.CustomerFK)
                    .Include(a => a.VendorFK)
                    .FirstOrDefault()!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<PurchaseOrder> ReadAll()
        {
            try
            {
                return _context.PurchaseOrders
                    .Include(a => a.SalesOrderFK)
                    .Include(a => a.CustomerFK)
                    .Include(a => a.VendorFK)
                    .OrderByDescending(a => a.OrderDate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.ModifiedDate = DateTime.Now;
                _context.PurchaseOrders.Update(purchaseOrder);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int CreatePONumber(int accountId)
        {
            try
            {
                // get last PO number
                var lastPO = _context.PurchaseOrders.OrderBy(a => a.Id).Where(a => a.PurchaseOrderNumber != null).LastOrDefault();
                if (lastPO == null)
                {
                    return 10001;
                }
                else
                {
                    return lastPO!.PurchaseOrderNumber!.Value + 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<POEmailInterface>> ReadPOEmails(int? poNumber, ApplicationUser user)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync("mail.saleshelper.com", 143, false);
                await _imapClient.AuthenticateAsync(_context.Account.Find(user.AccountNumber)!.SaleshelperMailingAddress,
                                                    _encdecService.Decrypt(_context.Account.Find(user.AccountNumber)!.EncryptedSaleshelperEmailPassword!));

                List<POEmailInterface> inboxMessages = await ReadInboxEmails(_imapClient, poNumber);
                List<POEmailInterface> sentMessages = await ReadSentEmails(_imapClient, poNumber);
                List<POEmailInterface> allMessages = new();
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

        private async Task<List<POEmailInterface>> ReadInboxEmails(ImapClient _imapClient, int? poNumber)
        {
            var inboxFolder = await _imapClient.GetFolderAsync("INBOX");
            // open the inbox folder
            await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
            // get emails from inbox folder with subject containing the PO number
            var messages = await inboxFolder.SearchAsync(SearchQuery.SubjectContains(poNumber.ToString()));
            List<POEmailInterface> emailMessages = new();
            foreach (var message in messages)
            {
                var mimeMessage = await inboxFolder.GetMessageAsync(message);
                var poEmailMessage = new POEmailInterface
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                SetupEmailAttachments(mimeMessage, poEmailMessage);
                emailMessages.Add(poEmailMessage);
            }
            return emailMessages;
        }

        private async Task<List<POEmailInterface>> ReadSentEmails(ImapClient _imapClient, int? poNumber)
        {
            var sentFolder = await _imapClient.GetFolderAsync("Sent");
            // open the sent folder
            await sentFolder.OpenAsync(FolderAccess.ReadOnly);
            // get emails from sent folder with subject containing the PO number
            var messages = await sentFolder.SearchAsync(SearchQuery.SubjectContains(poNumber.ToString()));
            List<POEmailInterface> emailMessages = new();
            foreach (var message in messages)
            {
                var mimeMessage = await sentFolder.GetMessageAsync(message);
                var poEmailMessage = new POEmailInterface
                {
                    MessageId = mimeMessage.MessageId,
                    From = mimeMessage.From.ToString(),
                    To = mimeMessage.To.ToString(),
                    Subject = mimeMessage.Subject,
                    Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                    Date = mimeMessage.Date.DateTime
                };
                SetupEmailAttachments(mimeMessage, poEmailMessage);
                emailMessages.Add(poEmailMessage);
            }
            return emailMessages;
        }

        private void SetupEmailAttachments(MimeMessage mimeMessage, POEmailInterface emailMessage)
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
                    emailMessage.Attachments.Add(new POAttachmentModel
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
                    emailMessage.Attachments.Add(new POAttachmentModel
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
