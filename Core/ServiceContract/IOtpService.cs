using Core.Entities;

namespace Core.ServiceContract
{
    public interface IOtpService
    {
        Task GenerateOtpAsync(string userId );
        Task ResetPasswordAsync(string userId, string code, string newPassword);
        Task ResetPhoneAsync(string userId, string code, string newPhoneNumber);
        Task SendTemporaryPasswordAsync(string userId, string phoneNumber,string email);


    }
}
