using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeReviews.Models
{
    public class Account
    {
        public Account()
        {
            Recipe = new HashSet<Recipe>();
        }

        public string AccountId { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Username must be shorter than 30 characters.")]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
        public DateTime Created { get; set; }

        [MaxLength(200, ErrorMessage = "The description cannot be longer than 200 characters.")]
        public string Description { get; set; }

        public string PictureFilename { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }

        public virtual ICollection<RatingHistory> Ratings { get; set; }
    }
}
