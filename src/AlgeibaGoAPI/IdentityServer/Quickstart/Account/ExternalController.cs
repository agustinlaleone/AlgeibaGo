using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerAspNetIdentity.Migrations;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace Host.Quickstart.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ApplicationDbContext _dbContext;
        private readonly IClientStore _clientStore;
        private readonly IConfiguration _configuration;
        private readonly IEventService _events;
        PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>(
    new OptionsWrapper<PasswordHasherOptions>(
        new PasswordHasherOptions()
        {
            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
        })
);

        public ExternalController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            SignInManager<ApplicationUser> signInManager,

            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events)
        {
            _configuration = configuration;
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
        }

        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                throw new Exception("invalid return URL");
            }

            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                return await ProcessWindowsLoginAsync(returnUrl);
            }
            else
            {
                var props = new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(Callback)),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
                };

                return Challenge(props, provider);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("idsrv.external");
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);

            if (user == null)
            {
                return View("~/Views/Account/NoPermission.cshtml");
                //if (_configuration.GetValue<bool>("createUser"))
                //    user = await AutoProvisionUserAsync(provider, providerUserId, claims);
                //else
                //{
                //    this.ReturnError("Usuario no registrado");
                //}
            }

            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.PreferredUserName)?.Value ?? user.Id;
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name));
            await HttpContext.SignInAsync(user.Id, name, provider, localSignInProps, additionalLocalClaims.ToArray());

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var returnUrl = result.Properties.Items["returnUrl"];
            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("Callback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", AccountOptions.WindowsAuthenticationSchemeName },
                    }
                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await HttpContext.SignInAsync(
                    IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return Redirect(props.RedirectUri);
            }
            else
            {
                return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            var userIdClaim = externalUser.FindFirst(ClaimTypes.Email) ?? externalUser.FindFirst(JwtClaimTypes.Name) ??
                              externalUser.FindFirst(ClaimTypes.Name) ??
                              throw new Exception("Unknown userid");

            var email = externalUser.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
              externalUser.FindFirst(x => x.Type == ClaimTypes.Email)?.Value ??
              externalUser.FindFirst(x => x.Type == ClaimTypes.Upn)?.Value;

            var username = externalUser.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName)?.Value ??
              externalUser.FindFirst(x => x.Type == ClaimTypes.Name)?.Value;

            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            var identity = externalUser.Claims.ToArray();

            var user = _dbContext.Users.Where(x => x.UserName == username || x.Email == email).FirstOrDefault();

            if (user == null)
            {
                bool createUser = _configuration.GetValue<bool>("createUser");
                string rolDefault = _configuration.GetValue<string>("roleDefault");
                string passwordDefault = _configuration.GetValue<string>("passwordDefault");

                if (createUser)
                {
                    ApplicationUser newUser = new ApplicationUser();
                    newUser.Email = email;
                    newUser.UserName = username;
                    newUser.PasswordHash = passwordDefault;
                    await _userManager.CreateAsync(newUser);
                    var passHash = hasher.HashPassword(newUser, passwordDefault);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                    await _userManager.ResetPasswordAsync(newUser, token, passHash);
                    await _userManager.AddToRoleAsync(newUser, rolDefault);

                    user = newUser;
                }
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    user = null; // no lo dejamos loguear
                }
            }
            
            return (user, provider, providerUserId, claims);
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var filtered = new List<Claim>();

            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var nameUser = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName)?.Value ??
              claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };
            var identityResult = await _userManager.CreateAsync(user, "Provisional@2019");

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, _configuration.GetValue<string>("roleDefault"));
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
                identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            }

            return user;
        }
        private IActionResult Redirect()
        {
            return Redirect("http://localhost:4200");
        }


        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }

        private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }

        private IActionResult ReturnError(string messageDescription)
        {
            var vm = new ErrorViewModel();

            var message = messageDescription;
            if (message != null)
            {
                ErrorMessage messages = new ErrorMessage();
                messages.Error = message;
                vm.Error = messages;
            }

            return View("Error", vm);
        }
    }
}