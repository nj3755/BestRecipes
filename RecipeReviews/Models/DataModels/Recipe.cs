using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeReviews.Models
{
    public class Recipe
    {

        public Recipe()
        {
            RecipeTag = new HashSet<RecipeTag>();
        }

        public int RecipeId { get; set; }
        public string AccountId { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "The title is too long, it must be shorter than 80 characters.")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "The recipe is too long, it must be shorter than 500 characters.")]
        public string Text { get; set; }

        public DateTime Created { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        public string ImageFilename { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<RecipeTag> RecipeTag { get; set; }
        public virtual ICollection<RatingHistory> Ratings { get; set; }
    }
}
