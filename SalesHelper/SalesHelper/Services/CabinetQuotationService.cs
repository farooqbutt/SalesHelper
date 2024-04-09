using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services.EncryptionServices;

namespace SalesHelper.Services
{
    public class CabinetQuotationService : ICabinetQuotationService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerService _customerService;
        private readonly EncryptionDecryptionService _encdecService;
        public CabinetQuotationService(ApplicationDbContext context, CustomerService customerService, EncryptionDecryptionService encdecService)
        {
            _context = context;
            _customerService = customerService;
            _encdecService = encdecService;
        }

        public void Create(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Add(cabinetQuotation);
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
                _context.CabinetQuotations.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CabinetQuotation Read(int id)
        {
            try
            {
                return _context.CabinetQuotations.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CabinetQuotation> ReadAll()
        {
            try
            {
                return _context.CabinetQuotations.Include(a => a.VendorIdFk).Include(a => a.CustomerIdFk).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Update(cabinetQuotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CabinetQuoteInterface CabinetQuoteInterface(CabinetQuotation cabinetQuotation)
        {

            var cabinetQuoteInterface = new CabinetQuoteInterface
            {
                Id = cabinetQuotation.Id,
                Name = cabinetQuotation.Name,
                CustomerId = cabinetQuotation.CustomerId,
                CustomerIdFk = _customerService.Read(cabinetQuotation.CustomerId),
                VendorId = cabinetQuotation.VendorId,
                VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!,
                Construction = cabinetQuotation.Construction,
                BoxMaterials = cabinetQuotation.BoxMaterials,
                isOneColorDesign = cabinetQuotation.isOneColorDesign,
                // one color
                WoodSpeciesForOneColorDesign = cabinetQuotation.WoodSpeciesForOneColorDesign,
                DoorStyleForOneColorDesign = cabinetQuotation.DoorStyleForOneColorDesign,
                CabinetFinishForOneColorDesign = cabinetQuotation.CabinetFinishForOneColorDesign,
                // multiple color
                UpperWoodSpeciesForMultipleColorDesign = cabinetQuotation.UpperWoodSpeciesForMultipleColorDesign,
                UpperDoorStyleForMultipleColorDesign = cabinetQuotation.UpperDoorStyleForMultipleColorDesign,
                UpperFinishForMultipleColorDesign = cabinetQuotation.UpperFinishForMultipleColorDesign,
                LowerWoodSpeciesForMultipleColorDesign = cabinetQuotation.LowerWoodSpeciesForMultipleColorDesign,
                LowerDoorStyleForMultipleColorDesign = cabinetQuotation.LowerDoorStyleForMultipleColorDesign,
                LowerFinishForMultipleColorDesign = cabinetQuotation.LowerFinishForMultipleColorDesign,
                IslandWoodSpeciesForMultipleColorDesign = cabinetQuotation.IslandWoodSpeciesForMultipleColorDesign,
                IslandFinishForMultipleColorDesign = cabinetQuotation.IslandFinishForMultipleColorDesign,
                IslandDoorStyleForMultipleColorDesign = cabinetQuotation.IslandDoorStyleForMultipleColorDesign,
                CommentsOnMultiColorDesign = cabinetQuotation.CommentsOnMultiColorDesign,
                // additional information
                AdditionalInformation = cabinetQuotation.AdditionalInformation,
                CreatedByUserId = cabinetQuotation.CreatedByUserId,
                CreatedDateTime = cabinetQuotation.CreatedDateTime,
                ModifiedDateTime = cabinetQuotation.ModifiedDateTime,
                CabinetPrice = cabinetQuotation.CabinetPrice,
                DeliveryCharge = cabinetQuotation.DeliveryCharge,
                InstallationFee = cabinetQuotation.InstallationFee,
                Tax = cabinetQuotation.Tax,
                VendorPrice = cabinetQuotation.VendorPrice,
                CommentOnPrice = cabinetQuotation.CommentOnPrice,
                Refrigerator = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Refrigerator"],
                StoveAndCooktop = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["StoveAndCooktop"],
                Dishwasher = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Dishwasher"],
                Hood = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Hood"],
                BuiltInOven = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["BuiltInOven"],
                BuiltInDrawerMicrowave = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["BuiltInDrawerMicrowave"],
                Sink = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Sink"],
                Comments = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Comments"]
            };
            return cabinetQuoteInterface;
        }

        public async Task<List<QuoteEmailInterface>> ReadCabinetQuoteEmails(int quoteId, ApplicationUser user)
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
            var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId && qe.QuoteType == "Cabinet Quote").Select(qe => qe.EmailMessageId).ToList();
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
            var emailIds = _context.QuotationEmails.Where(qe => qe.QuotationId == quoteId && qe.QuoteType == "Cabinet Quote").Select(qe => qe.EmailMessageId).ToList();

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
    }
}
