using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Taraweb.Models;
using Newtonsoft.Json;
using Taraweb.Models.TarawebM1;
using Taraweb.Middleware.ModelM4s;
using System.Text;
using Taraweb.Middleware.Models;

namespace Taraweb.Controllers
{
    [Route("Account/[action]")]
    public partial class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;
            this.configuration = configuration;
        }

        private IActionResult RedirectWithError(string error, string redirectUrl = null)
        {
             if (!string.IsNullOrEmpty(redirectUrl))
             {
                 return Redirect($"~/cms/Login?error={error}&redirectUrl={Uri.EscapeDataString(redirectUrl)}");
             }
             else
             {
                 return Redirect($"~/cms/Login?error={error}");
             }
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model, string redirectUrl)
        {
            //if (env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
            //{
            //    var claims = new List<Claim>()
            //    {
            //        new Claim(ClaimTypes.Name, "neabadmin"),
            //        new Claim(ClaimTypes.Email, "neabemailadmin"),
            //        new Claim(ClaimTypes.Role,"admin")
            //    };

            //    roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
            //    await signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = userName, Email = userName }, isPersistent: false, claims);

            //    return Redirect($"~/{redirectUrl}");
            //}

            if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://119.59.114.151:8001/api/User",data);
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string content = await response.Content.ReadAsStringAsync();
                        User user = JsonConvert.DeserializeObject<User>(content);
                        // Do something with the response content
                        Console.WriteLine(content);
                    
                        if(user.Id != 0) { 
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.LoginName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role,"admin")
                        };


                        await signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = user.LoginName, Email = user.Email }, isPersistent: false, claims);

                        return Redirect($"~/{redirectUrl}");
                        }
                    }
                }
               

               
            }

            return RedirectWithError("Invalid user or password", redirectUrl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Invalid password");
            }

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(id);

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(message);
        }

        [HttpPost]
        public ApplicationAuthenticationState CurrentUser()
        {
            return new ApplicationAuthenticationState
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name,
                Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
            };
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Redirect("~/");
        }
    }
}