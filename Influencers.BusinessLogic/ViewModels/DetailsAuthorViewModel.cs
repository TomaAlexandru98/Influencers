using Influencers.BusinessLogic.VIewModels;
using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.BusinessLogic.ViewModels
{
    public class DetailsAuthorViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Votes { get; set; }
        public string ImageSource { get; set; }
        public string Description { get; set; }
        public IEnumerable<ArticleViewModel> Articles { get; set; }
    }
}
