namespace ProductService.Domain.Events
{
    public class ProductCreatedEvent
    {
        public long ProductId { get; set; }
        public string ProductUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string TitleDomestic { get; set; } = string.Empty;
        public string DescriptionDomestic { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string OtherCode { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        // Diğer gerekli alanlar eklenebilir
    }
}