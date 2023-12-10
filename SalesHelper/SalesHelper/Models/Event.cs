using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string? End { get; set; } = string.Empty;
        public bool IsAllDay { get; set; }
        public string? Color { get; set; } = string.Empty;

        [ForeignKey("CustomerIdFK")]
        public int CustomerId { get; set; }
        public Customer CustomerIdFK { get; set; } = default!;
    }
}
