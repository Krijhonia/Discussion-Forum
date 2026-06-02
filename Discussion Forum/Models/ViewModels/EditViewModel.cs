using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscussionForum.Models.ViewModels;

public class EditPostViewModel
{
        public Post Post { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        [ValidateNever]
        public IFormFile? FeatureImage { get; set; }
}

