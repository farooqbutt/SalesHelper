namespace SalesHelper.Models
{
    public class CountertopBrandsData
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string? Link { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string? Collection { get; set; } = string.Empty;
        public string? Series { get; set; } = string.Empty;
        public string? Group { get; set; } = string.Empty;
        public string? Finish { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Thickness { get; set; } = string.Empty;
        public string? SlabSize { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;

        // Material
        public string Material { get; set; } = string.Empty;
    }
}
