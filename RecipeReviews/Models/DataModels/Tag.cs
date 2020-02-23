using System;
using System.Collections.Generic;

namespace RecipeReviews.Models
{
    public partial class Tag
    {
        public Tag()
        {
            RecipeTag = new HashSet<RecipeTag>();
        }

        public int TagId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RecipeTag> RecipeTag { get; set; }

    }
}
