using RecipeReviews.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RecipeReviews.Services
{
    public class RecipeService : DataAccessService<Recipe, int>
    {

        public RecipeService(RecipeReviewsContext context) : base(context)
        {
        }

        /// <summary>
        /// Add a recipe and other required entries.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="tagNames"></param>
        /// <returns><see cref="DbContext.SaveChangesAsync"/>.</returns>
        public async Task<int> Add(Recipe recipe, IEnumerable<string> tagNames)
        {
            var findTags = _context.Tag
                .Where(t => tagNames.Contains(t.Name))
                .ToListAsync();

            _context.Add(recipe);

            var tags = await findTags;

            if (tags.Any())
            {
                var recipeTags = tags
                    .Select(t => new RecipeTag() { Recipe = recipe, Tag = t, TagId = t.TagId, RecipeId = recipe.RecipeId });
                _context.RecipeTag.AddRange(recipeTags);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public int UpdateRating(Recipe recipe, double rating, out double parsedRating)
        {
            parsedRating = recipe.Rating > 0 ? (recipe.Rating + rating) / 2 : rating;
            recipe.Rating = parsedRating;

            var rh = new RatingHistory()
            {
                AccountId = recipe.AccountId,
                RecipeId = recipe.RecipeId,
                Account = recipe.Account,
                Recipe = recipe
            };
            _context.Add(rh);
            _context.Update(recipe);
            return _context.SaveChanges();
        }


        public override async Task<int> Delete(Recipe recipe)
        {
            var rT = _context.RecipeTag.Where(x => x.RecipeId == recipe.RecipeId).ToList();
            if (rT.Any())
            {
                _context.RecipeTag.RemoveRange(rT);
            }
            return await base.Delete(recipe);
        }

        public async Task<IEnumerable<Recipe>> All()
        {
            return await _context.Recipe
                .Include(r => r.Account)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="Recipe"/> objects and transforms each to a new object./>
        /// </summary>
        /// <param name="selection">The selection for the LINQ Select() call.</param>
        /// <returns>The new <see cref="IEnumerable{T}"/>.</returns>
        public async Task<IEnumerable<T>> All<T>(Expression<Func<Recipe, T>> selection)
        {
            return await _context.Recipe
                .Include(r => r.Account)
                .Select(selection)
                .ToListAsync();
        }

        public IEnumerable<Recipe> AllOrderedByDescending<TKey>(Func<Recipe, TKey> ordering)
        {
            return _context.Recipe
                .Include(r => r.Account)
                .OrderByDescending(ordering);
        }

        /// <summary>
        /// Gets the accounts of the recipes in an <see cref="IEnumerable{Recipe}"/>
        /// along with the number of recipes per account.
        /// </summary>
        /// <param name="recipes">The recipes.</param>
        /// <param name="numberOfAccounts">The number of accounts to get.</param>
        /// <returns>The recipes per account in a <see cref="List{KeyValuePair}"/>.</returns>
        public List<KeyValuePair<Account, int>> GetAccountsWithRecipeCount(IEnumerable<Recipe> recipes, int numberOfAccounts)
        {
            return recipes
                .GroupBy(r => r.Account)
                .Select(group => new KeyValuePair<Account, int>(group.Key, group.Count()))
                .OrderByDescending(x => x.Value)
                .Take(numberOfAccounts)
                .ToList();
        }

        public IEnumerable<string> GetTagNames() => _context.Tag.Select(t => t.Name).ToList();


        /// <summary>
        /// Gets all <see cref="Recipe"/> objects and transforms each to a new object./>
        /// </summary>    
        /// <param name="condition">The selection for the LINQ Where() call.</param>
        /// <param name="selection">The selection for the LINQ Select() call.</param>
        /// <returns>The new <see cref="IEnumerable{T}"/>.</returns>
        public async Task<IEnumerable<T>> GetRecipesWhere<T>(
            Expression<Func<Recipe, bool>> condition,
            Expression<Func<Recipe, T>> selection)
        {
            return await _context.Recipe
                .Where(condition)
                .Include(r => r.Account)
                .Select(selection)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for <see cref="Recipe"/> objects by the query and returns results of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query string.</param>
        /// <param name="selection">Func for selecting recipes with LINQ Select().</param>
        /// <returns></returns>
        public IEnumerable<T> GetSearchResults<T>(string query, Func<Recipe, T> selection)
        {
            if (query.Length < 3)
            {
                return _context.Recipe.Select(selection).ToList();
            }

            string value = query.ToLower();
            var recipeModels = new List<T>();
            var words = query.Split();

            for (int i = 0; i < (words.Length < 5 ? words.Length : 5); i++)
            {
                recipeModels.AddRange(_context.RecipeTag
                    .Where(x => x.Tag.Name.Contains(words[i]) || words[i].Contains(x.Tag.Name))
                    .Select(x => x.Recipe)
                    .Include(r => r.Account)
                    .Select(selection)
                    .ToArray());
            }

            if (!recipeModels.Any())
            {
                recipeModels.AddRange(_context.Recipe
                    .Where(r => r.Title.StartsWith(query))
                    .Select(selection)
                    .ToList());
            }
            if (!recipeModels.Any())
            {
                recipeModels.AddRange(_context.Recipe
                    .Where(r => r.Title.Contains(query))
                    .Select(selection)
                    .ToList());
            }
            return recipeModels;
        }

        public async Task<Account> FindAccount(string accountId)
        {
            return await _context.Account.FindAsync(accountId);
        }

        public async Task<Recipe> FindRecipe(int recipeId)
        {
            return await _context.Recipe
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.RecipeId == recipeId);
        }

        public async Task<List<Tag>> FindTags(Recipe recipe)
        {
            return await _context.RecipeTag.Where(x => x.RecipeId == recipe.RecipeId).Select(x => x.Tag).ToListAsync();
        }

    }
}
