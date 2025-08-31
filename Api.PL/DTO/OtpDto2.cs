namespace Api.PL.DTO
{
    public class OtpDto2
    {
        public string Code { get; set; }
       
        public DateTime ExpirationTime { get; set; } // Added ExpirationTime for better validation
    }
}
