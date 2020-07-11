using Influencers.BusinessLogic.VIewModels;
using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.BusinessLogic.ViewModels
{
    public class DetailsArticleViewModel
    {
        public ArticleViewModel CurrentArticle { get; set; }
        public IEnumerable<ArticleViewModel> RelatedArticles { get; set; }
    }
}
