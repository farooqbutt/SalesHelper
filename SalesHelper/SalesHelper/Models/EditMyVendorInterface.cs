namespace SalesHelper.Models
{
    public class EditMyVendorInterface
    {
        public int VendorId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? MainPhone { get; set; } = string.Empty;
        public string? Fax { get; set; } = string.Empty;
        public string? PrimaryContact { get; set; } = string.Empty;
        public string? PrimaryContactPhone { get; set; } = string.Empty;
        public string? Website { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int BusinessTypeId { get; set; }
        public bool IsPrivate { get; set; }
        public int? BusinessAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int AccountNumberId { get; set; } 

        public List<Address> AddressDetails { get; set; } = default!;

        public int? VendorReferenceId { get; set; }

        public VendorDocuments VendorDocuments { get; set; } = default!;
        public IFormFile VendorFile { get; set; } = default!;

        public List<VendorDocuments>? VendorDocumentsList { get; set; }
    }
}
