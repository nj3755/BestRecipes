using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeReviews.Models;
using RecipeReviews.Services;

namespace RecipeReviews.Controllers
{
    public class HomeController : Controller
    {

        public static string WelcomeMessageKey { get; } = "welcomeKey";

        public async Task<IActionResult> Index([FromServices] RecipeService recipeService)
        {
            IEnumerable<Recipe> recipes = recipeService.AllOrderedByDescending(r => r.Created);

            List<RecipeViewModel> recipeModels = recipes
                .Select(r => r.ToRecipeViewModel())
                .Take(15)
                .ToList();

            foreach (var item in recipeModels)
            {
                var tags = await recipeService.FindTags(item.Recipe);
                item.TagNames = tags.Select(t => t.Name);
            }

            List<KeyValuePair<Account, int>> membersWithRecipeCount = recipeService
                .GetAccountsWithRecipeCount(recipes, 10);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var welcomeMsg = HttpContext.Session.GetString(WelcomeMessageKey);
                if (welcomeMsg != null)
                {
                    ViewData["WelcomeMsg"] = welcomeMsg;
                }
            }
            return View(new IndexViewModel(membersWithRecipeCount, recipeModels));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult Search(IFormCollection collection)
        {
            if (collection != null)
            {
                string value = collection["Search"].ToString().Trim();
                if (value.Any())
                {
                    return RedirectToAction(nameof(RecipesController.Index), "Recipes", new { q = value });
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Members([FromServices] AccountService service)
        {
            return View(await service.All());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            ViewBag.Code = statusCode;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
