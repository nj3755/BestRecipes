using RecipeReviews.Models;
using RecipeReviews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Controllers
{
    public class RecipesController : Controller
    {
        private readonly RecipeService _recipeService;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly FileUploadService _fileUploadService;

        public RecipesController(RecipeReviewsContext context, IHostingEnvironment hostingEnv, FileUploadService uploadService)
        {
            _recipeService = new RecipeService(context);
            _hostingEnv = hostingEnv;
            _fileUploadService = uploadService;
        }

        public static string ImageId { get; } = "imageElement";
        public static string ImageErrorId { get; } = "imageError";
        public static string SubmitImageErrorId { get; } = "submitImageError";

        public static string CreationSubmitErrorId { get; } = "submitImageError";
        public static string SubmitTextSuccessId { get; } = "submitDescSuccess";
        public static string SubmitTextErrorId { get; } = "submitDescError";

        // GET: Recipes
        public async Task<IActionResult> Index(string q, string sort, string accountId)
        {
            IEnumerable<RecipeViewModel> recipeViewModels;

            if (accountId != null)
            {
                var getRecipeModels = _recipeService.GetRecipesWhere(r => r.AccountId == accountId, r => r.ToRecipeViewModel());
                var findAccount = _recipeService.FindAccount(accountId);
                
                var account = await findAccount;
                ViewData["Username"] = account != null ? account.Username : accountId;
                recipeViewModels = await getRecipeModels;
            }
            else
            {
                if (q != null)
                {
                    ViewData["Query"] = q;
                    recipeViewModels = _recipeService.GetSearchResults(q, r => r.ToRecipeViewModel());
                }
                else
                {
                    recipeViewModels = await _recipeService.All(r => r.ToRecipeViewModel());
                }

                if (sort != null && Sorting.RecipeOptions.ContainsKey(sort))
                {
                    recipeViewModels = recipeViewModels.Sort(Sorting.RecipeOptions[sort]);
                }
            }
            return View(recipeViewModels.ToList());
        }

        [Authorize]
        public IActionResult RecipeImage()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                                    "StaticFiles", "images", "banner1.svg");

            return PhysicalFile(file, "image/svg+xml");
        }

        [HttpPost]
        public IActionResult Sort(string q, IFormCollection input)
        {
            var sortingInput = input["recipeSortingOptions"].ToString();

            if (sortingInput != Sorting.OptionLabel)
            {
                return RedirectToAction(nameof(Index), new { q, sort = sortingInput });
            }
            return RedirectToAction(nameof(Index), new { q });
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var tagNames = _recipeService.GetTagNames();
                return View(new RecipeCreationViewModel(tagNames));
            }
            return Unauthorized();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Text,Image,TagNames")] RecipeCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var recipe = new Recipe();
                await SetNewRecipeProperties(recipe, model);

                if (await _recipeService.Add(recipe, model.TagNames).Success())
                {
                    if (model.Image != null)
                    {
                        if (FileIsInvalid(model.Image, out JsonResult jsonResult))
                        {
                            return jsonResult;
                        }
                         
                        // If it fails, ignore and let the user upload it again later 
                        await _fileUploadService.TryUpload(model.Image, recipe.ImageFilename, _hostingEnv);
                    }
                    return Json(new {
                        success = true,
                        message = "",
                        redirectLocation = $"/recipes/details/{recipe.RecipeId}" });
                }
            }
            return Json(new {
                success = false,
                message = "The recipe could not be saved due to an error. Please try again later.",
                elementId = CreationSubmitErrorId });
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _recipeService.FindRecipe((int)id);

            if (recipe == null)
            {
                return NotFound();
            }

            var tags = await _recipeService.FindTags(recipe);

            return View(new RecipeViewModel()
            {
                Recipe = recipe,
                ImageSrc = Program.GetRequestPath(recipe.ImageFilename),
                TagNames = tags.Select(t => t.Name.Trim()),
                ViewerIsRecipeOwner = HttpContext.GetAccountId() == recipe.AccountId
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRating(string rating, string recipeId)
        {
            Recipe recipe = null;

            if (HttpContext.User.Identity.IsAuthenticated
                && double.TryParse(rating, out double parsedRating)
                && int.TryParse(recipeId, out int id))
            {
                recipe = await _recipeService.FindRecipe(id);

                if (recipe != null)
                {
                    var account = await _recipeService.FindAccount(recipe.AccountId);

                    if (!_recipeService.AnyRatingHistory(account, recipe)
                        && _recipeService.UpdateRating(recipe, parsedRating, out double newRating).Success())
                    {
                        return Json(new { success = true, rating = newRating.ToString() });
                    }
                }
            }
            return Json(new { success = false, rating = recipe != null ? recipe.Rating.ToString() : rating });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecipeText(string text, int recipeId)
        {
            var findRecipeTask = _recipeService.FindRecipe(recipeId);
            var defaultFailureJson = Json(new { success = false, message = "", elementId = SubmitImageErrorId });

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return defaultFailureJson;
            }

            var recipe = await findRecipeTask;

            if (recipe == null)
            {
                return defaultFailureJson;
            }

            recipe.Text = text;

            if (await _recipeService.Update(recipe).Success())
            {
                return Json(new
                {
                    success = true,
                    message = "Text updated!",
                    elementId = SubmitTextSuccessId,
                    errorElementIds = new string[] { SubmitTextErrorId }
                });
            }

            return Json(new { success = false, message = "The text could not be updated due to an error. Please try again later.", elementId = SubmitTextErrorId });
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage([FromForm(Name = "Image")] IFormFile file, int recipeId)
        {
            var defaultFailureJson = Json(new { success = false, message = "", elementId = SubmitImageErrorId });
            var accountId = HttpContext.GetAccountId();
            var findRecipe = _recipeService.FindRecipe(recipeId);

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return defaultFailureJson;
            }

            if (file != null && FileIsInvalid(file, out JsonResult jsonResult))
            {
                return jsonResult;
            }

            var recipe = await findRecipe;
            if (recipe == null)
            {
                return defaultFailureJson;
            }
            _fileUploadService.DeleteIfNotNull(recipe.ImageFilename, _hostingEnv);

            var message = "";
            var fileName = GenerateImageFilename(accountId, file);

            if (await _fileUploadService.TryUpload(file, fileName, _hostingEnv))
            {
                recipe.ImageFilename = fileName;

                if (await _recipeService.Update(recipe).Success())
                {
                    return Json(new
                    {
                        success = true,
                        message,
                        elementId = ImageId,
                        imageSrc = Program.GetRequestPath(recipe.ImageFilename),
                        errorElementIds = new string[] { ImageErrorId }
                    });
                }
                else
                {
                    recipe.ImageFilename = null;
                }
            }
            message = "The picture could not be saved due to an error. Please try again later.";
            return Json(new { success = false, message, elementId = SubmitImageErrorId });
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id, string error)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var recipe = await _recipeService.FindRecipe((int)id);
            if (recipe == null)
            {
                return NotFound();
            }

            if (error != null)
            {
                ViewBag.Error = error;
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _recipeService.FindRecipe(id);
            if (await _recipeService.Delete(recipe).Success())
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete), new { error = "Failed to delete the recipe" });
        }

        private string GenerateImageFilename(string accountId, IFormFile file)
        {
            var s = file.FileName.Split('.');
            return $"r-img-{accountId}-{DateTime.Now.ToString("yyyyMMddhhmmss")}." + s[s.Length - 1];
        }

        private bool FileIsInvalid(IFormFile file, out JsonResult errorJson)
        {
            if (_fileUploadService.InvalidExtension(file))
            {
                errorJson = Json(new { success = false, message = "Only JPG and PNG images are allowed.", elementId = ImageErrorId });
                return true;
            }
            if (_fileUploadService.InvalidNameLength(file))
            {
                errorJson = Json(new { success = false, message = "The filename is too short.", elementId = ImageErrorId });
                return true;
            }

            if (_fileUploadService.InvalidSize(file))
            {
                errorJson = Json(new { success = false, message = "The file can't be larger than 5 MB.", elementId = ImageErrorId });
                return true;
            }
            errorJson = null;
            return false;
        }

        private string ReformatNewRecipeText(string text) => text.Replace("\n", "<br />");

        private async Task<bool> SetNewRecipeProperties(Recipe recipe, RecipeCreationViewModel model)
        {
            recipe.AccountId = HttpContext.GetAccountId();
            var findAccount = _recipeService.FindAccount(recipe.AccountId);
            recipe.Created = DateTime.Now;
            recipe.Rating = 0;
            recipe.Text = ReformatNewRecipeText(model.Text);
            recipe.Title = model.Title;
            if (model.Image != null)
            {
                recipe.ImageFilename = GenerateImageFilename(recipe.AccountId, model.Image);
            }
            recipe.Account = await findAccount;
            return true;
        }
    }
}
