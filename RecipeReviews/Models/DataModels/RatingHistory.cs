using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeReviews.Models
{
    public class RatingHistory
    {
        public string AccountId { get; set; }

        public int RecipeId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
