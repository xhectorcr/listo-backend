namespace ListoAPI.DTO
{
    public class QrValidationRequestDTO
    {
        public int UserId { get; set; }
        public int CartId { get; set; }
        public long ExpirationTimestamp { get; set; } 
        public string HashSignature { get; set; }
    }
}