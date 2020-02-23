using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeReviews.Models
{
    public class RecipeCreationViewModel
    {
        public RecipeCreationViewModel()
        {

        }
        public RecipeCreationViewModel(IEnumerable<string> tagNames)
        {
            TagNames = tagNames;
        }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(3000)]
        public string Text { get; set; }

        public IEnumerable<string> TagNames { get; set; }


        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
    }
}
