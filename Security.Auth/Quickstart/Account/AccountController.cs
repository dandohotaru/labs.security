using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Labs.Security.Auth.Quickstart.Shared.Attributes;
using Labs.Security.Domain.Features.Users;
using Labs.Security.Domain.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Labs.Security.Auth.Quickstart.Account
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        public AccountController(
            IIdentityServerInteractionService interactionService,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService eventsService,
            UserStore usersStore)
        {
            UsersStore = usersStore;
            InteractionService = interactionService;
            EventsService = eventsService;
            AccountService = new AccountService(interactionService, httpContextAccessor, clientStore);
        }

        private IIdentityServerInteractionService InteractionService { get; }

        private IEventService EventsService { get; }

        private AccountService AccountService { get; }

        private UserStore UsersStore { get; }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await AccountService.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // only one option for logging in
                return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (UsersStore.ValidateCredentials(model.Username, model.Password))
                {
                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    }

                    // issue authentication cookie with subject ID and username
                    var user = UsersStore.FindByUsername(model.Username);
                    await EventsService.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));
                    await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, props);

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint or a local page
                    if (InteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                await EventsService.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await AccountService.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var context = await InteractionService.GetAuthorizationContextAsync(returnUrl);

            returnUrl = Url.Action("ExternalLoginCallback", new {returnUrl = returnUrl});

            // windows authentication is modeled as external in the asp.net core authentication manager, so we need special handling
            if (AccountOptions.WindowsAuthenticationSchemes.Contains(provider))
            {
                // but they don't support the redirect uri, so this URL is re-triggered when we call challenge
                if (HttpContext.User is WindowsPrincipal principal)
                {
                    var nameWithoutDomain = principal.Identity.Alias();

                    var props = new AuthenticationProperties();
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationProviderName);

                    var claims = new ClaimsIdentity(provider);
                    claims.AddClaim(new Claim(JwtClaimTypes.Subject, nameWithoutDomain));
                    claims.AddClaim(new Claim(JwtClaimTypes.Name, HttpContext.User.Identity.Name));

                    // add the groups as claims -- be careful if the number of groups is too large
                    if (AccountOptions.IncludeWindowsGroups)
                    {
                        var identity = principal.Identity as WindowsIdentity;
                        if (identity != null && identity.Groups != null)
                        {
                            var groups = identity.Groups.Translate(typeof(NTAccount));
                            if (groups != null)
                            {
                                var roles = groups.Select(p => new Claim(JwtClaimTypes.Role, p.Value));
                                claims.AddClaims(roles);
                            }
                        }
                    }

                    // deal with context
                    //if (context.IdP != null)
                    //{
                    //    // ToDo: [DanD]
                    //    var user = new
                    //    {
                    //        SubjectId = HttpContext.User.Identity.Name,
                    //        Username = HttpContext.User.Identity.Name,
                    //    };

                    //    claims.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, context.IdP));
                    //    var method = OidcConstants.AuthenticationMethods.WindowsIntegratedAuthentication;

                    //    var scheme = HttpContext.Authentication.GetAuthenticationSchemes();
                    //    var temp = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    //    await HttpContext.Authentication.SignInAsync(temp, user.SubjectId, user.Username, provider, new []{ method }, props, claims.Claims.ToArray());
                    //}
                    //else
                    {
                        await HttpContext.Authentication.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, new ClaimsPrincipal(claims), props);
                    }

                    return Redirect(returnUrl);
                }

                // this triggers all of the windows auth schemes we're supporting so the browser can use what it supports
                return new ChallengeResult(AccountOptions.WindowsAuthenticationSchemes);
            }
            else
            {
                // start challenge and roundtrip the return URL
                var props = new AuthenticationProperties
                {
                    RedirectUri = returnUrl,
                    Items = {{"scheme", provider}}
                };
                return new ChallengeResult(provider, props);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var context = await InteractionService.GetAuthorizationContextAsync(returnUrl);

            // read external identity from the temporary cookie
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = tempUser.Claims.ToList();

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            //var provider = info.Properties.Items["scheme"];
            var provider = context.IdP;
            var userId = userIdClaim.Value;

            // check if the external user is already provisioned
            var user = UsersStore.FindByProvider(provider, userId);
            if (user == null)
            {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                user = UsersStore.ProvisionUser(provider, userId, claims);
            }

            var additionalClaims = new List<Claim>();

            // if the external system sent a session id claim, copy it over
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            AuthenticationProperties props = null;
            var id_token = info.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] {new AuthenticationToken {Name = "id_token", Value = id_token}});
            }

            // issue authentication cookie for user
            await EventsService.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.SubjectId, user.Username));
            await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, provider, props, additionalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            if (InteractionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await AccountService.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var vm = await AccountService.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                var url = Url.Action("Logout", new {logoutId = vm.LogoutId});
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties {RedirectUri = url});
                }
                catch (NotSupportedException) // this is for the external providers that don't have signout
                {
                }
                catch (InvalidOperationException) // this is for Windows/Negotiate
                {
                }
            }

            // delete local authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            var user = await HttpContext.GetIdentityServerUserAsync();
            if (user != null)
            {
                await EventsService.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetName()));
            }

            return View("LoggedOut", vm);
        }
    }

    public static class Extensions
    {
        public static async Task SignInAsync(this AuthenticationManager manager, string scheme, string sub, string name, string identityProvider, IEnumerable<string> authenticationMethods, AuthenticationProperties properties, params Claim[] claims)
        {
            await manager.SignInAsync(scheme, IdentityServerPrincipal.Create(sub, name, identityProvider, authenticationMethods, claims), properties);
        }
    }
}