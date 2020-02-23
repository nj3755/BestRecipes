using RecipeReviews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Services
{
    public static class Extensions
    {

        /// <summary>
        /// An indication of success after saving changes to the <see cref="RecipeReviewsContext"/>. 
        /// </summary>
        /// <param name="saveChangesResult">The save changes task.</param>
        /// <returns>True if it is greater than 0.</returns>
        public static bool Success(this int saveChangesTask) => saveChangesTask > 0;

        /// <summary>
        /// An indication of success after saving changes to the <see cref="RecipeReviewsContext"/>. 
        /// </summary>
        /// <param name="saveChangesResult">The save changes task.</param>
        /// <returns>True if it is greater than 0.</returns>
        public async static Task<bool> Success(this Task<int> saveChangesTask) => (await saveChangesTask).Success();

    }
}
