using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using RecipeReviews.Models;
using RecipeReviews.Services;

namespace RecipeReviews.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserAuthenticationService _authService;
        private readonly FileUploadService _fileUploadService;

        public AccountController(
            RecipeReviewsContext context,
            IHostingEnvironment hostingEnv,
            UserAuthenticationService authService,
            FileUploadService uploadService)
        {

            _accountService = new AccountService(context);
            _hostingEnv = hostingEnv;
            _authService = authService;
            _fileUploadService = uploadService;
        }

        #region Error Ids

        public static string ProfilePictureId { get; } = "imageElement";
        public static string ProfilePictureErrorId { get; } = "imageError";
        public static string SubmitPicErrorId { get; } = "submitImageError";
        public static string SubmitDescSuccessId { get; } = "submitDescSuccess";
        public static string SubmitDescErrorId { get; } = "submitDescError";
        public static string LoginErrorId { get; } = "loginError";
        public static string UsernameErrorId { get; } = "usernameError";
        public static string PasswordErrorId { get; } = "passwordError";
        public static string ChangeEmailSubmitErrorId { get; } = "emailError";
        public static string ChangeEmailSubmitSuccessId { get; } = "emailSuccess";
        public static string ChangePasswordSubmitErrorId { get; } = "passwordError";
        public static string ChangePasswordSuccessId { get; } = "passwordSuccess";

        #endregion

        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var account = await _accountService.Find(GenerateAccountId(username));

            if (_authService.Authenticate(account, password, out UserAuthenticationService.AuthenticationError error))
            {
                await _authService.SignIn(HttpContext, account);
                return Json(new { success = true, message = "" });
            }

            switch (error)
            {
                case UserAuthenticationService.AuthenticationError.InvalidUsername:
                    return Json(new { success = false, message = "Invalid username.", elementId = UsernameErrorId });
                case UserAuthenticationService.AuthenticationError.InvalidPassword:
                    return Json(new { success = false, message = "Incorrect password.", elementId = PasswordErrorId });
                default:
                    return Json(new { success = false, message = "Error while signing in, please try again later.", elementId = LoginErrorId });
            }

        }

        public async Task<IActionResult> SignOut()
        {
            await _authService.SignOut(HttpContext);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }



        public IActionResult AccessDenied()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // GET: Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Email")] Account account, string confirmPassword)
        {
            if (account.Password != confirmPassword)
            {
                ViewData["ConfirmPasswordError"] = "The passwords do not match.";
                return View(account);
            }

            if (ModelState.IsValid)
            {
                account.AccountId = GenerateAccountId(account.Username);
                account.Password = new PasswordHasher<Account>().HashPassword(account, account.Password);
                account.Created = DateTime.Now;

                if (_accountService.Any(account.AccountId))
                {
                    ViewData["UsernameError"] = "An account with that username already exists.";
                }
                else if (await _accountService.Add(account).Success())
                {
                    await _authService.SignIn(HttpContext, account);
                    SetSessionString(HomeController.WelcomeMessageKey, $"Welcome {account.Username}!");
                    return RedirectToAction(nameof(HomeController.Index), "Recipes");
                }
            }
            return View(account);
        }

        // GET: Profile/id
        public async Task<IActionResult> Profile(string id)
        {
            var countRecipes = _accountService.GetNumberOfRecipes(id);

            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            int rCount = await countRecipes;
            ViewData["NumberOfRecipesDesc"] = rCount > 1 ? $"{rCount} recipes" : $"{rCount} recipe";
            ViewData["ProfilePictureSrc"] = account.GetImageRequestPath();
            return View(account);
        }

        // GET: Details
        public async Task<IActionResult> Details()
        {
            var id = HttpContext.GetAccountId();
            if (!HttpContext.User.Identity.IsAuthenticated || id == null)
            {
                return Unauthorized();
            }

            var account = await _accountService.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            ViewData["AccountId"] = id;
            ViewData["NumberOfRecipes"] = await _accountService.GetNumberOfRecipes(account.AccountId);
            ViewData["ProfilePictureSrc"] = account.GetImageRequestPath();
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDescription(string description)
        {
            var accountId = HttpContext.GetAccountId();
            var account = await _accountService.Find(accountId);

            if (account == null || account.Description == description)
            {
                return Json(new { success = false, message = "", elementId = SubmitDescErrorId });
            }

            account.Description = description;

            if (await _accountService.Update(account).Success())
            {
                return Json(new
                {
                    success = true,
                    message = "Description updated!",
                    elementId = SubmitDescSuccessId,
                    errorElementIds = new string[] { SubmitDescErrorId }
                });
            }
            return Json(new { success = false, message = "The description could not be updated due to an error. Please try again later.", elementId = SubmitDescErrorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(string email)
        {
            var accountId = HttpContext.GetAccountId();
            var account = await _accountService.Find(accountId);

            if (account == null || account.Email == email)
            {
                return Json(new { success = false, message = "", elementId = ChangeEmailSubmitErrorId });
            }
            account.Email = email;

            if (await _accountService.Update(account).Success())
            {
                return Json(new
                {
                    success = true,
                    message = "Email updated!",
                    elementId = ChangeEmailSubmitSuccessId,
                    errorElementIds = new string[] { ChangeEmailSubmitErrorId }
                });
            }

            return Json(new {
                success = false,
                message = "The description could not be updated due to an error. Please try again later.",
                elementId = "editEmailError" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(string oldPassword, string password, string confirmPassword)
        {
            var accountId = HttpContext.GetAccountId();
            var account = await _accountService.Find(accountId);

            if (account != null)
            {
                if (password != confirmPassword)
                {
                    return Json(new { success = false, message = "The passwords do not match.", elementId = "confirmPasswordError" });
                }

                var authenticated = _authService.Authenticate(account, oldPassword, out UserAuthenticationService.AuthenticationError error);

                if (!authenticated)
                {
                    switch (error)
                    {
                        case UserAuthenticationService.AuthenticationError.InvalidPassword:
                            return Json(new { success = false, message = "Incorrect password.", elementId = "oldPasswordError" });
                        default:
                            return Json(new { success = false,
                                message = "The old password was correct but an error occured. Please try again later.",
                                elementId = ChangePasswordSubmitErrorId });
                    }
                }
                account.Password = new PasswordHasher<Account>().HashPassword(account, password);

                if (await _accountService.Update(account).Success())
                {
                    return Json(new
                    {
                        success = true,
                        message = "Password changed!",
                        elementId = ChangePasswordSuccessId,
                        errorElementIds = new string[] { PasswordErrorId, ChangePasswordSubmitErrorId, "oldPasswordError", "confirmPasswordError" }
                    });
                }
            }
            return Json(new { success = false, message = "The password could not be updated due to an error. Please try again later.", elementId = ChangePasswordSubmitErrorId });
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfilePicture([FromForm(Name = "Image")] IFormFile file)
        {
            var accountId = HttpContext.GetAccountId();
            var findAccount = _accountService.Find(accountId);

            if (FileIsInvalid(file, out JsonResult jsonResult))
            {
                return jsonResult;
            }

            var account = await findAccount;
            string fileName = GenerateImageFilename(account, file.FileName);
            _fileUploadService.DeleteIfNotNull(account.PictureFilename, _hostingEnv);

            if (await _fileUploadService.TryUpload(file, fileName, _hostingEnv))
            {
                account.PictureFilename = GenerateImageFilename(account, file.FileName);

                if (await _accountService.Update(account).Success())
                {
                    return Json(new
                    {
                        success = true,
                        message = "",
                        elementId = ProfilePictureId,
                        imageSrc = Program.GetRequestPath(account.PictureFilename),
                        errorElementIds = new string[] { ProfilePictureErrorId }
                    });
                }
                else
                {
                    account.PictureFilename = null;
                }
            }
            return Json(new
            {
                success = false,
                message = "The picture could not be saved due to an error. Please try again later.",
                elementId = SubmitPicErrorId
            });
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var account = await _accountService.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Account/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (await _authService.SignOut(HttpContext))
            {
                var account = await _accountService.Find(id);
                await _accountService.Delete(account);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return RedirectToAction(nameof(Delete), new { id });
        }

        private string GenerateAccountId(string username) => HttpUtility.UrlEncode(username, Encoding.ASCII).ToLower();

        private string GenerateImageFilename(Account a, string originalFilename)
        {
            var s = originalFilename.Split('.');
            return $"{a.AccountId.ToLower()}-profile-picture." + s[s.Length - 1];
        }

        private bool FileIsInvalid(IFormFile file, out JsonResult errorJson)
        {
            if (file == null)
            {
                errorJson = Json(new { success = false, message = "", elementId = ProfilePictureErrorId });
                return true;
            }
            if (_fileUploadService.InvalidExtension(file))
            {
                errorJson = Json(new { success = false, message = "Only JPG and PNG images are allowed.", elementId = ProfilePictureErrorId });
                return true;
            }
            if (_fileUploadService.InvalidNameLength(file))
            {
                errorJson = Json(new { success = false, message = "The filename is too short.", elementId = ProfilePictureErrorId });
                return true;
            }

            if (_fileUploadService.InvalidSize(file))
            {
                errorJson = Json(new { success = false, message = "The file can't be larger than 5 MB.", elementId = ProfilePictureErrorId });
                return true;
            }
            errorJson = null;
            return false;
        }

        private void SetSessionString(string key, string s) => HttpContext.Session.SetString(key, s);
    }
}
