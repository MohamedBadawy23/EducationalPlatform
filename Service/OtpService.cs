using Core.Entities;
using Core.ServiceContract;
using Core.Specefications;
using CoreProject.Repository;
using Microsoft.AspNetCore.Identity;
using RepositoryProject.Repository;
using Talabat.Core.Specefications;
 

public class OtpService : IOtpService
{
    private readonly IGenericRepository<AppUser> _userRepository;
    private readonly IGenericRepository<OtpEntry> _otpRepository;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<AppUser> _userManager;

    public OtpService(
        IGenericRepository<AppUser> userRepository,
        IGenericRepository<OtpEntry> otpRepository,
        IEmailSender emailSender,
        UserManager<AppUser> userManager)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _emailSender = emailSender;
        _userManager = userManager;
    }

    // إنشاء OTP وإرساله على البريد
    public async Task GenerateOtpAsync(string userId)
    {
        // جلب المستخدم
        var userSpec = new UserByIdSpec(userId);
        var user = await _userRepository.GetByIdWithSpecAsync(userSpec);

        if (user == null)
            throw new Exception("User not found.");

        // إنشاء OTP عشوائي صالح لمدة 5 دقائق
        var otp = new OtpEntry
        {
            UserId = userId,
            Code = new Random().Next(100000, 999999).ToString(),
            Expiry = DateTime.UtcNow.AddMinutes(5)
        };

        await _otpRepository.AddAsync(otp);
        await _otpRepository.SaveChangeAsync();

        // إرسال OTP على البريد
        var subject = "رمز التحقق OTP لتغيير معلومات الحساب";
        var body = $"رمز التحقق الخاص بك هو: <b>{otp.Code}</b>. صالح لمدة 5 دقائق.";
        await _emailSender.SendEmailAsync(user.Email, subject, body);
    }

    // تغيير كلمة المرور بعد التحقق من OTP
    public async Task ResetPasswordAsync(string userId, string otpCode, string newPassword)
    {
        var user = await _userRepository.GetByIdWithSpecAsync(new UserByIdSpec(userId));
        if (user == null)
            throw new Exception("User not found.");

        var otp = await _otpRepository.GetByIdWithSpecAsync(new Specefication<OtpEntry>(
            o => o.UserId == userId && o.Code == otpCode && o.Expiry > DateTime.UtcNow));

        if (otp == null)
            throw new Exception("OTP غير صالح أو منتهي.");

        // تغيير كلمة المرور باستخدام UserManager
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new Exception("فشل تغيير كلمة المرور: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        // حذف OTP بعد الاستخدام
        _otpRepository.Delete(otp);
        await _otpRepository.SaveChangeAsync();
    }

    // تغيير رقم الهاتف بعد التحقق من OTP
    public async Task ResetPhoneAsync(string userId, string otpCode, string newPhoneNumber)
    {
        var user = await _userRepository.GetByIdWithSpecAsync(new UserByIdSpec(userId));
        if (user == null)
            throw new Exception("User not found.");

        var otp = await _otpRepository.GetByIdWithSpecAsync(new Specefication<OtpEntry>(
            o => o.UserId == userId && o.Code == otpCode && o.Expiry > DateTime.UtcNow));

        if (otp == null)
            throw new Exception("OTP غير صالح أو منتهي.");

        // تحديث رقم الهاتف
        user.PhoneNumber = newPhoneNumber;
        await _userRepository.SaveChangeAsync();

        // حذف OTP بعد الاستخدام
        _otpRepository.Delete(otp);
        await _otpRepository.SaveChangeAsync();
    }
    public async Task SendTemporaryPasswordAsync(string email, string phone, string tempPassword)
    {
         
        var subject = "بيانات الدخول إلى المنصة التعليمية";
        var message = $@"
        مرحباً 👋
        
        تم إنشاء حسابك بنجاح.
        
        ✉️ البريد الإلكتروني: {email}
        📱 رقم الهاتف: {phone}
        🔑 كلمة المرور المؤقتة: {tempPassword}
        
        برجاء تغيير كلمة المرور عند أول تسجيل دخول.
    ";

        await _emailSender.SendEmailAsync(email, subject, message);
    }
}