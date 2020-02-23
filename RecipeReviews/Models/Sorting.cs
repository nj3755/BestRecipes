using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Models
{
    public static class Sorting
    {
        private static readonly string titleDesc = "Title (Descending)";
        private static readonly string titleAsc = "Title (Ascending)";
        private static readonly string mostRecent = "Most Recent";
        private static readonly string leastRecent = "Least Recent";
        private static readonly string rating = "Rating";

        public static Dictionary<string, RecipeOption> RecipeOptions { get; } = new Dictionary<string, RecipeOption>()
        {
            { titleAsc, RecipeOption.TitleAsc },
            { titleDesc, RecipeOption.TitleDesc },
            { rating, RecipeOption.Rating },
            { mostRecent, RecipeOption.MostRecent },
            { leastRecent, RecipeOption.LeastRecent }
        };

        public static string OptionLabel { get; } = "Sort";

        public enum RecipeOption
        {
            TitleDesc,
            TitleAsc,
            Rating,
            MostRecent,
            LeastRecent
        }
  
    }
}
