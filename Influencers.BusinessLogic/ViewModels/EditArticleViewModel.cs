using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Influencers.BusinessLogic.ViewModels
{
    public class EditArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public IEnumerable<string> TagsDb { get; set; }

        [Display(Name = "Tags")]
        [Required]
        public string ArticleTags { get; set; }
    }
}
