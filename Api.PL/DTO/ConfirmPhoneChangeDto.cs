namespace Api.PL.DTO
{
    public class ConfirmPhoneChangeDto
    {
        public string UserId { get; set; }
        public string NewPhoneNumber { get; set; }
        public string OtpCode { get; set; }
    }
}
