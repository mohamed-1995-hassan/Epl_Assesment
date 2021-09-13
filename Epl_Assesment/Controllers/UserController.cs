using Epl_Assesment.Model;
using Epl_Assesment.Model.Data;
using Epl_Assesment.Model.Dtos;
using Epl_Assesment.Model.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epl_Assesment.Controllers
{
    [ApiController]
    [EnableCors("MyPolicy")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtConfig _jwtConfig;

        public UserController(ILogger<UserController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtConfig> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtConfig = options.Value;
        }
        [HttpPost]
        [Route("api/UserController/RegisterUser")]
        public async Task<object> RegisterUser([FromBody] NewUSer newUSer) {

            try
            {

                var user = new AppUser() { UserName = newUSer.UserName, Email = newUSer.Email };
                var result = await _userManager.CreateAsync(user, newUSer.Password);
                if (result.Succeeded)
                {
                    return new { result = "added successfully" };
                }
                return new { result = "register failed" };
            }
            catch (Exception ex)
            {
                return new { result = ex.Message };
            }

        }

        [HttpGet]
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/UserController/GetAllUsers")]
        public async Task<object> GetAllUsers()
        {
            try
            {
                var users = _userManager.Users.Select(x => new UserInfo(x.Email, x.UserName));
                return await Task.FromResult(users);
            }
            catch (Exception ex)
            {
                return new { resulte = ex.Message };
            }
        }
        [HttpPost]
        [Route("api/UserController/LoginUser")]
        public async Task<object> LoginUser([FromBody] LoggedUser loggedUser)
        {
            try
            {
                if (loggedUser.UserName == "" || loggedUser.Password == "")
                {
                    return await Task.FromResult("parameter are missing");
                }
                var result = await _signInManager.PasswordSignInAsync(loggedUser.UserName, loggedUser.Password, false, false);
                if (result.Succeeded)
                {
                    var appuser = await _userManager.FindByNameAsync(loggedUser.UserName);
                    
                    var user = new UserInfo(appuser.Email, appuser.UserName);
                    user.Token = GenerateToken(appuser);
                    return await Task.FromResult(user);
                }
                return new { result = "Login failed" };
            }
            catch (Exception ex)
            {
                return new { result = ex.Message };
            }

        }

        private string GenerateToken(AppUser user)
        {
            var jwtTokenHandeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] {

                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtConfig.Audience,
                Issuer = _jwtConfig.Issuser
            };
            var token = jwtTokenHandeler.CreateToken(tokenDescriptor);
            return jwtTokenHandeler.WriteToken(token);

        }
        [HttpGet]
        [Route("api/UserController/balance")]
        public async Task<IActionResult> WalletOPerations(string addedBalanceEmail, string SubtractedBalanceEmail, double amount)
        {
         var userSub = await _userManager.FindByEmailAsync(SubtractedBalanceEmail);
         var userAdd = await _userManager.FindByEmailAsync(addedBalanceEmail);

         if(userSub.balance>= amount)
            {
                userSub.balance -= amount;
                userAdd.balance += amount;
            }
           await _userManager.UpdateAsync(userAdd);
           return Ok(userAdd.balance);
        }

    }
}
