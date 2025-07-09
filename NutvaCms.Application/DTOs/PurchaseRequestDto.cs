namespace NutvaCms.Application.DTOs
{
    public class PurchaseRequestDto
    {
        public string BuyerName { get; set; }           // Name
        public string Phone { get; set; }               // Phone number
        public string Region { get; set; }              // Region
        public string Comment { get; set; }             // Additional information

        public List<PurchaseProductDto> Products { get; set; } = new(); // Order items (kept)
    }
}
