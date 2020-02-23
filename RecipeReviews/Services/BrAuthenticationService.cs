using RecipeReviews.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RecipeReviews.Services
{
    public class BrAuthenticationService
    {
        public bool Authenticate(Account account, string password, out AuthenticationError error)
        {
            if (account == null)
            {
                error = AuthenticationError.InvalidUsername;
                return false;
            }

            error = AuthenticationError.None;
            var username = account.Username;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                error = AuthenticationError.InvalidInput;
                return false;
            }

            var result = new PasswordHasher<Account>().VerifyHashedPassword(account, account.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                error = AuthenticationError.InvalidPassword;
                return false;
            }
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                error = AuthenticationError.DatabaseError;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="account">The account</param>
        /// <returns>True if the sign-in was successful</returns>
        public async Task<bool> SignIn(HttpContext context, Account account)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, account.AccountId));
            identity.AddClaim(new Claim(ClaimTypes.Name, account.Username));
            var principal = new ClaimsPrincipal(identity);
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });
            return context.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="account">The account</param>
        /// <returns>True if the sign-out was successful</returns>
        public async Task<bool> SignOut(HttpContext context)
        {
            await context.SignOutAsync();
            return context.User.Identity.IsAuthenticated;
        }

        public enum AuthenticationError
        {
            InvalidInput,
            InvalidUsername,
            InvalidPassword,
            DatabaseError,
            None
        }
    }
}
