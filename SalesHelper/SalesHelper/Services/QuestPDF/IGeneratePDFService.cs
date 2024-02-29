using System.Security.Claims;

namespace SalesHelper.Services.QuestPDF
{
    public interface IGeneratePDFService
    {
        public byte[] GenerateCabinetQuoteEstimateRequestPDF(int id, ClaimsPrincipal user);
        public byte[] GenerateCountertopQuoteEstimateRequestPDF(int id, int vendorId, ClaimsPrincipal user);
    }
}
