using System;
using System.Collections.Generic;

namespace RecipeReviews.Models
{
    public partial class RecipeTag
    {
        public int RecipeId { get; set; }
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
