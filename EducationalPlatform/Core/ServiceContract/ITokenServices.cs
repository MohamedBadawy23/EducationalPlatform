
using Core.Entities;
using CoreProject.Enteties;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface ITokenServices
    {
        Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager);
        //Task<string> CreateToken(Student user, UserManager<Student> userManager);
    }
}
