using RecipeReviews.Models;
using RecipeReviews.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeReviews.Controllers
{
    public static class Extensions
    {
        public static string GenerateImageId(this Account uploader)
        {
            return $"{DateTime.Now.ToString("yyyy-MM-dd_hh:MM:ss.ffff")}_{uploader.AccountId}";
        }

        public static string GetImageRequestPath(this Account account)
        {
            return account.PictureFilename != null 
                ? Program.GetRequestPath(account.PictureFilename)
                : "/images/default-profile-picture.png";
        }

        public static string GetAccountId(this HttpContext context)
        {
            var claim = context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            return claim?.Value;
        }

        public static IEnumerable<RecipeViewModel> Sort(this IEnumerable<RecipeViewModel> models, Sorting.RecipeOption option)
        {
            switch (option)
            {
                case Sorting.RecipeOption.MostRecent:
                    models = models.OrderByDescending(r => r.Recipe.Created).ToList();
                    break;
                case Sorting.RecipeOption.TitleAsc:
                    models = models.OrderBy(r => r.Recipe.Title).ToList();
                    break;
                case Sorting.RecipeOption.TitleDesc:
                    models = models.OrderByDescending(r => r.Recipe.Title).ToList();
                    break;
                case Sorting.RecipeOption.Rating:
                    models = models.OrderByDescending(r => r.Recipe.Rating).ToList();
                    break;
                case Sorting.RecipeOption.LeastRecent:
                    models = models.OrderBy(r => r.Recipe.Created).ToList();
                    break;
                default:
                    break;
            }
            return models;
        }

        public static RecipeViewModel ToRecipeViewModel(this Recipe recipe)
        {
            return new RecipeViewModel()
            {
                Recipe = recipe,
                ImageSrc = Program.GetRequestPath(recipe.ImageFilename)
            };
        }
    }

}
