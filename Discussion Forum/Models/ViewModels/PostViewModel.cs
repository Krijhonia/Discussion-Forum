using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscussionForum.Models.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        [ValidateNever]
        public IFormFile? FeatureImage { get; set; }
    }
}
