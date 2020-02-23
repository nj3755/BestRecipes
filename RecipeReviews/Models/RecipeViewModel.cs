using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Models
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }

        public string ImageSrc { get; set; }

        public IEnumerable<string> TagNames { get; set; }

        public bool ViewerIsRecipeOwner { get; set; }

    }
}
