namespace SalesHelper.Models
{
    public class QuotationItem
    {
        public int Id { get; set; }
        public string Item { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Hinge { get; set; } = string.Empty;
        public string Finish { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuotationId { get; set; }
    }
}
