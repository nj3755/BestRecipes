using RecipeReviews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecipeReviews.Services
{
    public class TagService
    {

        private readonly RecipeReviewsContext _context;

        public TagService(RecipeReviewsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of <see cref="Tag"/> names.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{string}"/> of names.</returns>
        public IEnumerable<string> GetTagNames()
        {
            return _context.Tag.Select(t => t.Name);
        }

        /// <summary>
        /// Finds tags with matching names to those in the argument.
        /// </summary>
        /// <param name="tagNames">An <see cref="IEnumerable{string}"/> containing a list of <see cref="Tag.Name"/>.</param>
        /// <returns>A list of <see cref="Tag"/> objects.</returns>
        public IEnumerable<Tag> Find(IEnumerable<string> tagNames)
        {
            return _context.Tag.Where(t => tagNames.Contains(t.Name));
        }
    }
}
