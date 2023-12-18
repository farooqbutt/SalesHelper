namespace SalesHelper.Models.InterfaceModels
{
    public class CustomerListInterface
    {
        public Customer CustomerObject { get; set; } = new Customer();
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
