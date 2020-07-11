using System;
using System.Collections.Generic;

namespace Influencers.Models
{
    public partial class ArticleTag : DataEntity
    {
        public int? ArticleId { get; set; }
        public int? TagId { get; set; }

        public virtual Article Article { get; set; }
        public virtual Tag Tag { get; set; }
        public static ArticleTag Create(Article article, Tag tag)
        {
            return new ArticleTag
            {
                Article = article,
                Tag = tag
            };
        }
    }
}
