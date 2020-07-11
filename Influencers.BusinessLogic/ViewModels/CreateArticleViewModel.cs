using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Influencers.BusinessLogic.VIewModels
{
    public class CreateArticleViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public bool DoesAuthorExist { get; set; }
        public IEnumerable<string> TagsDb { get; set; }

        [Display(Name = "Tags")]
        [Required]
        public string ArticleTags { get; set; }

        [Display(Name = "Article Image")]
        public IFormFile Image { get; set; }
    }
}
