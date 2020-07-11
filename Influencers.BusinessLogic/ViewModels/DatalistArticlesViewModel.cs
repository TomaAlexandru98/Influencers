using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.BusinessLogic.ViewModels
{
    public class DatalistArticlesViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }
}
