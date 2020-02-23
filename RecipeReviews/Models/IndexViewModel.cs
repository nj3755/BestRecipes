using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<KeyValuePair<Account, int>> membersWithRecipeCount, IEnumerable<RecipeViewModel> recipes)
        {
            MembersWithRecipeCount = membersWithRecipeCount;
            Recipes = recipes;
        }

        public IEnumerable<KeyValuePair<Account, int>> MembersWithRecipeCount { get; set; }

        public IEnumerable<RecipeViewModel> Recipes { get; set; }

    }
}
