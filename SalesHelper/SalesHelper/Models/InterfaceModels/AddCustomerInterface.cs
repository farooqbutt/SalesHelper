namespace SalesHelper.Models.InterfaceModels
{
    public class AddCustomerInterface
    {
        public Customer Customer { get; set; } = default!;
        public Address Address { get; set; } = default!;
    }
}
