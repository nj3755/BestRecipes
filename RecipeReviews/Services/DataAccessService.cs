using RecipeReviews.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Services
{
    public abstract class DataAccessService<T, TKey> where T : class
    {
        protected readonly RecipeReviewsContext _context;

        public DataAccessService(RecipeReviewsContext context)
        {
            _context = context;
        }

        public virtual async Task<int> Add(T item)
        {
            _context.Add(item);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> Update(T item)
        {
            _context.Update(item);
            return await _context.SaveChangesAsync();
        }


        public virtual async Task<int> Delete(T item)
        {
            _context.Remove(item);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<T> Find(TKey id)
        {
            return await _context.FindAsync<T>(id);
        }

        public bool AnyRatingHistory(Account account, Recipe recipe)
        {
            return _context.RatingHistory
                .Any(x => x.RecipeId == recipe.RecipeId && x.AccountId == account.AccountId);
        }
    }
}
