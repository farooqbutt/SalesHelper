using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using System.Text.Json;
using SalesHelper.Services.EncryptionServices;
using System.Text.RegularExpressions;

namespace SalesHelper.Services
{
    public class CountertopQuotationService : ICountertopQuotationService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionDecryptionService _encdecService;
        public CountertopQuotationService(ApplicationDbContext context, EncryptionDecryptionService encdecService)
        {
            _context = context;
            _encdecService = encdecService;
        }
        public void Create(CountertopQuotation quotation)
        {
            try
            {
                _context.CountertopQuotations.Add(quotation);
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
                _context.CountertopQuotations.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CountertopQuotation Read(int id)
        {
            try
            {
                return _context.CountertopQuotations.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CountertopQuotation> ReadAll()
        {
            try
            {
                return _context.CountertopQuotations.Include(a => a.CustomerIdFk).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(CountertopQuotation quotation)
        {
            try
            {
                _context.CountertopQuotations.Update(quotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<string> GetBrandsForMaterial(string material)
        {
            try
            {
                return _context.CountertopBrandsData.Where(a => a.Material == material).Select(a => a.Brand).Distinct().ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CountertopBrandsData> GetColorsForBrand(string brand, string color)
        {
            try
            {
                if (color == null)
                {
                    return _context.CountertopBrandsData.Where(a => a.Brand == brand).ToList();
                }
                else
                {
                    return _context.CountertopBrandsData.Where(a => a.Brand == brand && a.ColorName.StartsWith(color)).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<QuoteEmailInterface>> ReadCountertopQuoteEmails(int quoteId, ApplicationUser user)
        {
            try
            {
                using var _imapClient = new ImapClient();
                await _imapClient.ConnectAsync("mail.saleshelper.com", 143, false);
                await _imapClient.AuthenticateAsync(_context.Account.Find(user.AccountNumber)!.SaleshelperMailingAddress,
                                                    _encdecService.Decrypt(_context.Account.Find(user.AccountNumber)!.EncryptedSaleshelperEmailPassword!));

                List<QuoteEmailInterface> inboxMessages = await ReadInboxEmails(_imapClient, quoteId);
                List<QuoteEmailInterface> sentMessages = await ReadSentEmails(_imapClient, quoteId);
                List<QuoteEmailInterface> allMessages = new();
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

        private async Task<List<QuoteEmailInterface>> ReadInboxEmails(ImapClient _imapClient, int quoteId)
        {
            var inboxFolder = await _imapClient.GetFolderAsync("INBOX");
            // open the inbox folder
            await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
            // get email ids from our database for the quote
            var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId && qe.QuoteType == "Countertop Quote").Select(qe => qe.EmailMessageId).ToList();
            if (emailIds.Any())
            {
                List<QuoteEmailInterface> emailMessages = new();
                foreach (var id in emailIds)
                {
                    var messages = await inboxFolder.SearchAsync(SearchQuery.Or(
                        SearchQuery.HeaderContains("References", id),
                        SearchQuery.HeaderContains("In-Reply-To", id)
                    ));

                    foreach (var message in messages)
                    {
                        var mimeMessage = await inboxFolder.GetMessageAsync(message);
                        var quoteEmailMessage = new QuoteEmailInterface
                        {
                            MessageId = mimeMessage.MessageId,
                            From = mimeMessage.From.ToString(),
                            To = mimeMessage.To.ToString(),
                            Subject = mimeMessage.Subject,
                            Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                            Date = mimeMessage.Date.DateTime
                        };
                        SetupEmailAttachments(mimeMessage, quoteEmailMessage);
                        emailMessages.Add(quoteEmailMessage);
                    }
                }
                return emailMessages;
            }
            else
            {
                return new List<QuoteEmailInterface>();
            }
        }

        private async Task<List<QuoteEmailInterface>> ReadSentEmails(ImapClient _imapClient, int quoteId)
        {
            var sentFolder = await _imapClient.GetFolderAsync("Sent");
            // open the sent folder
            await sentFolder.OpenAsync(FolderAccess.ReadOnly);
            // get email ids from our database for the quote
            var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId && qe.QuoteType == "Countertop Quote").Select(qe => qe.EmailMessageId).ToList();

            if (emailIds.Any())
            {
                List<QuoteEmailInterface> emailMessages = new List<QuoteEmailInterface>();
                foreach (var id in emailIds)
                {
                    var messages = await sentFolder.SearchAsync(SearchQuery.HeaderContains("Message-Id", id));

                    foreach (var message in messages)
                    {
                        var mimeMessage = await sentFolder.GetMessageAsync(message);
                        var quoteEmailMessage = new QuoteEmailInterface
                        {
                            MessageId = mimeMessage.MessageId,
                            From = mimeMessage.From.ToString(),
                            To = mimeMessage.To.ToString(),
                            Subject = mimeMessage.Subject,
                            Body = mimeMessage.HtmlBody != null ? mimeMessage.HtmlBody : mimeMessage.TextBody,
                            Date = mimeMessage.Date.DateTime
                        };
                        SetupEmailAttachments(mimeMessage, quoteEmailMessage);
                        emailMessages.Add(quoteEmailMessage);
                    }
                }
                return emailMessages;
            }
            else
            {
                return new List<QuoteEmailInterface>();
            }
        }

        private void SetupEmailAttachments(MimeMessage mimeMessage, QuoteEmailInterface emailMessage)
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
                    emailMessage.Attachments.Add(new QuoteAttachmentModel
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
                    emailMessage.Attachments.Add(new QuoteAttachmentModel
                    {
                        ContentType = part.ContentType.MimeType,
                        FileName = fileName,
                        Base64Data = base64
                    });
                }
            }
        }

        public List<SalesCountertopItem> GetCountertopItems(int quoteId)
        {
            try
            {
                var countertopItems = _context.CountertopMaterials.Where(cd => cd.CountertopQuotationId == quoteId)
                    .Include(a => a.VendorIdFk)
                    .Include(a => a.CountertopQuotationIdFk)
                    .ToList();

                List<SalesCountertopItem> salesCountertopItems = new();
                foreach (var item in countertopItems)
                {
                    // Use 0 if EstimateSquareFeet is null or cannot be parsed to an integer
                    int estimatedSquareFeet = 0;
                    if (!string.IsNullOrEmpty(item.CountertopQuotationIdFk?.EstimateSquareFeet))
                    {
                        int.TryParse(Regex.Replace(item.CountertopQuotationIdFk.EstimateSquareFeet, @"\D", ""), out estimatedSquareFeet);
                    }

                    // Use 0 if PriceQuote is null
                    decimal unitPrice = item.PriceQuote ?? 0;
                    decimal subtotal = unitPrice * estimatedSquareFeet;

                    salesCountertopItems.Add(new SalesCountertopItem
                    {
                        Material = item.Material,
                        Brand = item.Brand,
                        Color = item.Color,
                        VendorId = item.VendorId,
                        RoomType = item.CountertopQuotationIdFk!.RoomType,
                        EdgeProfile = item.CountertopQuotationIdFk.EdgeProfile,
                        EstimatedSquareFeet = estimatedSquareFeet,
                        UnitPrice = unitPrice,
                        Subtotal = subtotal,
                        CountertopQuotationId = item.CountertopQuotationId
                    });
                }
                return salesCountertopItems;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
