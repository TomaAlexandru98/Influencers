using System;
using System.Collections.Generic;

namespace Influencers.Models
{
    public partial class Tag : DataEntity
    {
        public Tag()
        {
            ArticleTag = new HashSet<ArticleTag>();
        }

        public string Name { get; set; }

        public virtual ICollection<ArticleTag> ArticleTag { get; set; }
        public static Tag Create(string name)
        {
            return new Tag
            {
                Name = name
            };
        }
    }
}
