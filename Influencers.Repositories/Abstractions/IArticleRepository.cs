using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.Repositories.Abstractions
{
    public interface IArticleRepository : IBaseRepository<Article>
    {
        IEnumerable<ArticleTag> AddMultipleArticleTag(IEnumerable<ArticleTag> articleTagList);
        IEnumerable<ArticleTag> GetArticleTags(int id);
        IEnumerable<Article> FilterByTagId(int tagId);
        IEnumerable<Article> FilterByTagName(string searchTag);
        void RemoveMultipleArticleTags(List<ArticleTag> articleTagsToRemove);
        IEnumerable<Article> GetAllForAuthor(int id);
        IEnumerable<Article> GetAllInLast30Days();
    }
}
