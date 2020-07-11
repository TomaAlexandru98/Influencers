using Influencers.Models;
using Influencers.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Influencers.Repositories.Repositories
{
    public class EFArticleRepository : EFBaseRepository<Article>, IArticleRepository
    {
        public EFArticleRepository(BloggingContext dbContext) : base(dbContext)
        {
        }

        public override Article GetById(int id)
        {
            return this.dbContext.Article
                                 .Where(article => article.Id == id)
                                 .Include(article => article.Author)
                                 .SingleOrDefault();
        }

        public override IEnumerable<Article> GetAll()
        {
            return this.dbContext.Article
                                 .Include(article => article.Author);
        }

        public IEnumerable<ArticleTag> AddMultipleArticleTag(IEnumerable<ArticleTag> articleTagList)
        {
            this.dbContext.ArticleTag.AddRange(articleTagList);
            this.dbContext.SaveChanges();
            return articleTagList;
        }

        public IEnumerable<ArticleTag> GetArticleTags(int id)
        {
            return dbContext.ArticleTag.Include(articleTag => articleTag.Article)
                                       .Include(articleTag => articleTag.Tag)
                                       .Where(articleTag => articleTag.Article.Id == id);
        }

        public IEnumerable<Article> FilterByTagId(int tagId)
        {
            var articleTagList = dbContext.ArticleTag.Include(articleTag => articleTag.Article)
                                                     .ThenInclude(articleTag => articleTag.Author)
                                                     .Include(articleTag => articleTag.Tag)
                                                     .Where(articleTag => articleTag.Tag.Id == tagId);
            var articlesList = new List<Article>();
            foreach (var articleTag in articleTagList)
            {
                articlesList.Add(articleTag.Article);
            }
            return articlesList;
        }

        public IEnumerable<Article> FilterByTagName(string searchTag)
        {
            var articleTagList = dbContext.ArticleTag.Include(articleTag => articleTag.Article)
                                                     .ThenInclude(articleTag => articleTag.Author)
                                                     .Include(articleTag => articleTag.Tag)
                                                     .Where(articleTag => articleTag.Tag.Name == searchTag);
            var articlesList = new List<Article>();
            foreach (var articleTag in articleTagList)
            {
                articlesList.Add(articleTag.Article);
            }
            return articlesList;
        }

        public void RemoveMultipleArticleTags(List<ArticleTag> articleTagsToRemove)
        {
            dbContext.ArticleTag.RemoveRange(articleTagsToRemove);
            dbContext.SaveChanges();
        }

        public IEnumerable<Article> GetAllForAuthor(int id)
        {
            return dbContext.Article.Include(article => article.Author)
                                    .Where(article => article.Author.Id == id);
        }

        public IEnumerable<Article> GetAllInLast30Days()
        {
            return dbContext.Article.Include(article => article.Author)
                    .Where(article => article.Date.Value.AddDays(30).CompareTo(DateTime.UtcNow) > 0);
        }
    }
}
