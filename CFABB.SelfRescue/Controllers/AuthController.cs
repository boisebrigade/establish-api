using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFABB.SelfRescue.Data;
using CFABB.SelfRescue.Models.AuthModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CFABB.SelfRescue.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller {

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;
    
        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext) {

            this.signInManager = signInManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        #region Login/Logout
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model) {
            if (!ModelState.IsValid) {
                return StatusCode(400);
            }

            ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) {
                return BadRequest("Username or password is incorrect");
            }

            if (!user.EmailConfirmed) {
                return BadRequest("Please confirm email before continuing");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager
                .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded) {
                return Ok();
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (result.IsLockedOut) {
                return StatusCode(429);
            }

            return BadRequest();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutUser() {
            await signInManager.SignOutAsync();
            return NoContent();
        }
        #endregion

        #region ExternalLogin
        /// <summary>
        /// Initiates the external login.
        /// </summary>
        /// <param name="provider">The name of the external provider being used for authentication.</param>
        /// <returns>IActionResult indicating the results of the logout.</returns>
        /// <response code="401">A challenge for the client to authenticate the user.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusCodeResult), 401)]
        public IActionResult ExternalLogin(string provider) {
            // Request a redirect to the external login provider.
            string redirectUrl = "account/externalLoginCallback";
            AuthenticationProperties properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// The method called from the external authentication provider when the user is authenticated.
        /// </summary>
        /// <param name="remoteError">An error code returned by the provider.</param>
        /// <returns>A redirect to the form or the user information screen.</returns>
        /// <response code="200">The authentication was successful and a user was created if necessary.</response>
        /// <response code="400">There was an error returned from the external provider, or another failure in the authentication pipeline.</response>
        /// <response code="429">The user's account is locked out.</response>
        /// <response code="401">Signing in the local account failed.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusCodeResult), 200)]
        [ProducesResponseType(typeof(StatusCodeResult), 400)]
        [ProducesResponseType(typeof(StatusCodeResult), 429)]
        [ProducesResponseType(typeof(StatusCodeResult), 401)]
        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null) {
            if (!string.IsNullOrEmpty(remoteError)) {
                return BadRequest(remoteError);
            }

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) {
                return BadRequest("Failed to get the external login information.");
            }

            var userName = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userName)) {
                return BadRequest("The email address was not returned from the external provider.");
            }

            //check for user, and create if one doesn't exist.
            ApplicationUser user = await userManager.FindByEmailAsync(userName);
            if (user == null) {
                user = new ApplicationUser {
                    UserName = userName,
                    Email = userName
                };
                await userManager.CreateAsync(user);
            }

            //Find the external login or create it.
            IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
            if (logins.Count(l => l.ProviderKey == info.ProviderKey) == 0) {
                IdentityResult loginResult = await userManager.AddLoginAsync(user, info);
                if (!loginResult.Succeeded) {
                    return BadRequest(loginResult.Errors);
                }
            }

            //sign the user in to the system.
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (signInResult.IsLockedOut) {
                return StatusCode(429);
            }

            if (!signInResult.Succeeded) {
                return Unauthorized();
            }

            string redirectUrl = "localhost:4200";

            return Redirect(redirectUrl);
        }

        #endregion

        #region Registration
        #endregion
    }
}