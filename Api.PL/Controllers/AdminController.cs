using Api.PL.Controllers;
using Api.PL.DTO;
using AutoMapper;
using Core.Entities;
using Core.Repository;
using CoreProject.Enteties;
using CoreProject.Repository;
using CoreProject.Specefications;
using EducationalPlatform.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Repository;
using RepositoryProject;
using System.Security.Claims;
using Talabat.Core.Specefications;
using Talabat.Repository;

namespace EducationalPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly EducationalPlatformDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Center> _repository;
        private readonly ITokenServices _tokenServices;

        public AdminController(EducationalPlatformDbContext context, IMapper mapper, UserManager<AppUser> userManager,IGenericRepository<Center> repository,ITokenServices tokenServices)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
           _repository = repository;
           _tokenServices = tokenServices;
        }
        [HttpGet("centers")]
        public async Task<IActionResult> GetCenters()
        {
            var Spec = new CenterWithDetailSpecefications();
            var ceneters = await _repository.GetAllWithSpecAsync(Spec);

            return Ok(_mapper.Map<List<CenterDto>>(ceneters));
        }
        [HttpGet("Center/{id}")]
        public async Task<IActionResult> GetCentersById()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
             
            var Spec = new CenterWithDetailSpecefications(Guid.Parse(userId));
            var ceneters = await _repository.GetByIdWithSpecAsync(Spec);
            
            if (ceneters == null)
                return NotFound();

            return Ok(_mapper.Map<CenterDto>(ceneters));
        }



        //[HttpPost("centers")]
        //public async Task<IActionResult> CreateCenter([FromBody] CenterDto centerDto)
        //{
        //    if (centerDto == null || string.IsNullOrEmpty(centerDto.Name) || string.IsNullOrEmpty(centerDto.PhoneNumber))
        //    {
        //        return BadRequest("Invalid center data.");
        //    }
        //    var password = Guid.NewGuid().ToString("N").Substring(0, 10) + "!aA1";
        //    var user = new AppUser
        //    {
        //        Id = Guid.NewGuid().ToString("N"),
        //        UserName = centerDto.PhoneNumber,
        //        PhoneNumber = centerDto.PhoneNumber,

        //        FullName = centerDto.Name,
        //        Role = UserRole.Center

        //    };

        //    var Result = await _userManager.CreateAsync(user, password);

        //    if (!Result.Succeeded)
        //    {
        //        var errors = Result.Errors.Select(e => e.Description);
        //        return BadRequest(new { errors = errors.ToList() });
        //    }
        //    //var center=_mapper.Map<Center>(centerDto);


        //    await _userManager.AddToRoleAsync(user, "Center");

        //    var center = new Center
        //    {

        //        CenterName = centerDto.Name,
        //        PhoneNumber = centerDto.PhoneNumber,
        //        Address = centerDto.Address,
        //        UserId = user.Id
        //    };

        //    await _repository.AddAsync(center);
        //    await _repository.SaveChangeAsync();

        //    return Ok(new
        //    {
        //        Message = "Center created successfully",
        //        PhoneNumber = centerDto.PhoneNumber,
        //        Password = password
        //    });


        //}

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateCenter")]
        public async Task<IActionResult> CreateCenter([FromBody] CenterDto centerDto)
        {
            if (centerDto == null || string.IsNullOrEmpty(centerDto.Name) || string.IsNullOrEmpty(centerDto.PhoneNumber))
                return BadRequest("Invalid center data.");

            var password = Guid.NewGuid().ToString("N").Substring(0, 10) + "!aA1";
            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString("N"),
                UserName = centerDto.PhoneNumber,
                PhoneNumber = centerDto.PhoneNumber,
                FullName = centerDto.Name,
                Role = UserRole.Center
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToList() });

            await _userManager.AddToRoleAsync(user, "Center");

            var center = new Center
            {
                CenterName = centerDto.Name,
                PhoneNumber = centerDto.PhoneNumber,
                Address = centerDto.Address,
                UserId = user.Id
            };

            await _repository.AddAsync(center);
            await _repository.SaveChangeAsync();

            // ? ‰Ê·¯œ «· Êﬂ‰ »⁄œ ≈‰‘«¡ «·‹ Center
            var tokens = await _tokenServices.GenerateTokensAsync(user);

            return Ok(new
            {
                Message = "Center created successfully",
                PhoneNumber = centerDto.PhoneNumber,
                Password = password,
                CenterId = center.Id,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCenter(Guid id)
        {
             
            var center = await _repository.GetByIdWithSpecAsync(new CenterWithDetailSpecefications(id));
            if (center == null)
                return NotFound("Center not found.");

            
            if (!string.IsNullOrEmpty(center.UserId))
            {
                var user = await _userManager.FindByIdAsync(center.UserId);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest("Failed to delete associated AppUser.");
                    }
                }
            }

             
            _repository.Delete(center);
            await _repository.SaveChangeAsync();

            return NoContent();
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCenter(Guid id)
        //{
        //    // 1- Ã·» «·”‰ — „⁄ «·‹ UserId
        //    var center = await _repository.GetByIdWithSpecAsync(new CenterWithDetailSpecefications(id));
        //    if (center == null)
        //        return NotFound("Center not found");

        //    // 2- Õ–› Õ”«» «·‹ AppUser ≈‰ ÊÃœ
        //    if (!string.IsNullOrEmpty(center.UserId))
        //    {
        //        var user = await _userManager.FindByIdAsync(center.UserId);
        //        if (user != null)
        //        {
        //            var result = await _userManager.DeleteAsync(user);
        //            if (!result.Succeeded)
        //                return BadRequest(result.Errors);
        //        }
        //    }

        //    // 3- Õ–› «·”‰ — ‰›”Â
        //      _repository.Delete(center);
        //    await _repository.SaveChangeAsync();

        //    return Ok("Center and associated user deleted successfully");
        //}




        private string GenerateCenterCode()
        {
            
            return new Random().Next(100000, 999999).ToString();
        }
    }
    } 