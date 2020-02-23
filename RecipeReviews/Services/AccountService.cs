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
    public class AccountService : DataAccessService<Account, string>
    {

        public AccountService(RecipeReviewsContext context) : base(context)
        {
        }

        public virtual async Task<IEnumerable<Account>> All()
        {
            return await _context.Account.ToListAsync();
        }

        public bool Any(string accountId)
        {
            return _context.Account.Any(x => x.AccountId == accountId);
        }

        public async Task<int> GetNumberOfRecipes(string accountId)
        {
            return await _context.Recipe.Where(r => r.AccountId == accountId).CountAsync();
        }
    }
}
