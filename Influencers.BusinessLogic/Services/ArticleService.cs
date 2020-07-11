
using Influencers.BusinessLogic.ViewModels;
using Influencers.BusinessLogic.VIewModels;
using Influencers.Models;
using Influencers.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Influencers.BusinessLogic.Services
{
    public class ArticleService
    {
        private readonly IArticleRepository articleRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly ITagRepository tagRepository;

        public ArticleService(IArticleRepository articleRepository,
                              IAuthorRepository authorRepository,
                              ITagRepository tagRepository)
        {
            this.articleRepository = articleRepository;
            this.authorRepository = authorRepository;
            this.tagRepository = tagRepository;
        }

        public IEnumerable<ArticleViewModel> GetAll()
        {
            var maxLengthArticle = 200;
            var articlesList = new List<ArticleViewModel>();
            foreach (var article in this.articleRepository.GetAll())
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    AuthorId = article.Author.Id,
                    Title = article.Title,
                    Content = GetStringWithLenghtLimit(article.Content, maxLengthArticle),
                    ImageSource = article.ImageSource,
                    ImageAuthorSource = article.Author.ImageSource,
                    Date = article.Date,
                    FirstName = article.Author.FirstName,
                    LastName = article.Author.LastName
                };
                articlesList.Add(articleViewModel);
            }
            return articlesList;
        }

        public ArticleViewModel GetById(int id)
        {
            var articleDb = articleRepository.GetById(id);
            var articleViewModel = new ArticleViewModel
            {
                Id = articleDb.Id,
                Content = articleDb.Content,
                Title = articleDb.Title,
                ImageSource = articleDb.ImageSource,
                Date = articleDb.Date,
                FirstName = articleDb.Author.FirstName,
                LastName = articleDb.Author.LastName,
                ImageAuthorSource = articleDb.Author.ImageSource,
                Email = articleDb.Author.Email,
                AuthorId = articleDb.Author.Id,
                Tags = MapArticleTagToTag(id)
            };
            return articleViewModel;
        }

        public DetailsArticleViewModel Details(int id)
        {
            var detailsArticleViewModel = new DetailsArticleViewModel
            {
                CurrentArticle = GetById(id),
                RelatedArticles = MapArticlesToArticlesViewModel(GetRelatedArticles(id))
            };
            return detailsArticleViewModel;
        }

        public Article Add(string title,
                           string content,
                           string email,
                           IFormFile image,
                           string imageDirectory,
                           IEnumerable<Tag> tags)
        {
            var authorDb = authorRepository.GetByEmail(email);
            string imageSource = null;
            if (image != null) imageSource = GetImagePath(imageDirectory, image);
            var articleToAdd = Article.Create(title, content, imageSource, authorDb);
            var articleTagList = CreateArticleTagList(articleToAdd, tags);
            articleRepository.Add(articleToAdd);
            articleRepository.AddMultipleArticleTag(articleTagList);
            return articleToAdd;
        }

        public Article Edit(int id, string title, string content, IEnumerable<Tag> articleTags)
        {
            var articleToEdit = articleRepository.GetById(id);
            RemoveArticleTags(id, articleTags);
            AddArticleTags(id, articleTags);
            articleToEdit.Update(title, content);
            return articleRepository.Update(articleToEdit);
        }

        private void RemoveArticleTags(int id, IEnumerable<Tag> articleTags)
        {
            var articleTagsToRemove = new List<ArticleTag>();
            foreach (var item in articleRepository.GetArticleTags(id))
            {
                if (!articleTags.Contains(item.Tag)) articleTagsToRemove.Add(item);
            }
            articleRepository.RemoveMultipleArticleTags(articleTagsToRemove);
        }

        private void AddArticleTags(int id, IEnumerable<Tag> articleTags)
        {
            var articleDb = articleRepository.GetById(id);
            var articleTagsToAdd = new List<ArticleTag>();
            foreach (var tag in articleTags)
            {
                bool alreadyBelong = false;
                foreach (var articleTagDb in articleRepository.GetArticleTags(id))
                {
                    if (articleTagDb.Tag == tag) alreadyBelong = true;
                }
                if (!alreadyBelong)
                {
                    var articleTagToCreate = ArticleTag.Create(articleDb, tag);
                    articleTagsToAdd.Add(articleTagToCreate);
                }
            }
            articleRepository.AddMultipleArticleTag(articleTagsToAdd);
        }

        public void Vote(int articleId, int voteValue)
        {
            var articleToVote = articleRepository.GetById(articleId);
            articleToVote.Author.GetVote(voteValue);
            authorRepository.Update(articleToVote.Author);
        }

        public IEnumerable<Tag> MapArticleTagToTag(int id)
        {
            var tagsList = new List<Tag>();
            foreach (var articleTag in articleRepository.GetArticleTags(id))
            {
                tagsList.Add(articleTag.Tag);
            }
            return tagsList;
        }

        public IEnumerable<ArticleViewModel> FilterByTagId(int id)
        {
            var maxLengthArticle = 200;
            var articleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articleRepository.FilterByTagId(id))
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    Content = GetStringWithLenghtLimit(article.Content, maxLengthArticle),
                    ImageSource = article.ImageSource,
                    ImageAuthorSource = article.Author.ImageSource,
                    Title = article.Title,
                    Date = article.Date,
                    FirstName = article.Author.FirstName,
                    LastName = article.Author.LastName,
                };
                articleViewModelList.Add(articleViewModel);
            }
            return articleViewModelList;
        }

        public void SendView(int id)
        {
            var articleDb = articleRepository.GetById(id);
            articleDb.IncreaseViews();
            articleRepository.Update(articleDb);
        }

        public IEnumerable<ArticleViewModel> GetTranding()
        {
            var articleInLast30Days = articleRepository.GetAllInLast30Days()
                                                       .OrderBy(article => article.GetTrendingCoefficient());
            var maxLengthArticle = 200;
            var articlesList = new List<ArticleViewModel>();
            foreach (var article in articleInLast30Days)
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    AuthorId = article.Author.Id,
                    Title = article.Title,
                    Content = GetStringWithLenghtLimit(article.Content, maxLengthArticle),
                    ImageSource = article.ImageSource,
                    ImageAuthorSource = article.Author.ImageSource,
                    Date = article.Date,
                    FirstName = article.Author.FirstName,
                    LastName = article.Author.LastName,
                };
                articlesList.Add(articleViewModel);
            }
            return articlesList;
        }

        public IEnumerable<ArticleViewModel> FilterByTagName(string searchTag)
        {
            if (searchTag == null) return new List<ArticleViewModel>();

            var maxLengthArticle = 200;
            var articleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articleRepository.FilterByTagName(searchTag))
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    Content = GetStringWithLenghtLimit(article.Content, maxLengthArticle),
                    Title = article.Title,
                    Date = article.Date,
                    FirstName = article.Author.FirstName,
                    LastName = article.Author.LastName,
                };
                articleViewModelList.Add(articleViewModel);
            }
            return articleViewModelList;
        }

        private IEnumerable<ArticleTag> CreateArticleTagList(Article article, IEnumerable<Tag> tags)
        {
            var articleTagList = new List<ArticleTag>();
            foreach (var tag in tags)
            {
                var articleTag = ArticleTag.Create(article, tag);
                articleTagList.Add(articleTag);
            }
            return articleTagList;
        }

        private string GetStringWithLenghtLimit(string inputString, int maxLengthString)
        {
            if (inputString.Length < maxLengthString) return inputString;
            return inputString.Substring(0, maxLengthString - 3) + "...";
        }

        private string GetImagePath(string uploadDir, IFormFile image)
        {
            string fileName = null;
            fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return fileName;
        }

        private IEnumerable<(Article, int)> ReturnSortArticlesAfterNoTags(int id)
        {
            var articleDb = articleRepository.GetById(id);
            var tagsForArticleDb = MapArticleTagToTag(id);
            var articleTagsTuple = new List<(Article, int)>();
            foreach (var article in articleRepository.GetAll().ToList())
            {
                var counter = 0;
                foreach (var articleTag in articleRepository.GetArticleTags(article.Id).ToList())
                {
                    if (tagsForArticleDb.Contains(articleTag.Tag)) counter++;
                }
                if (counter != 0 && article != articleDb) articleTagsTuple.Add((article, counter));
            }
            return articleTagsTuple.OrderByDescending(item => item.Item2);
        }
        
        private IEnumerable<ArticleViewModel> MapArticlesToArticlesViewModel(IEnumerable<Article> articles)
        {
            var articlesViewModelList = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content.Length < 200 ? article.Content : article.Content.Substring(0, 197) + "...",
                    FirstName = article.Author.FirstName,
                    LastName = article.Author.LastName
                };
                articlesViewModelList.Add(articleViewModel);
            }
            return articlesViewModelList;
        }

        private IEnumerable<Article> GetRelatedArticles(int id)
        {
            var numberOfArticlesRelated = 9;
            var iterator = 0;
            var articleTagTuple = ReturnSortArticlesAfterNoTags(id);
            var articlesRelated = new List<Article>();
            while (iterator < numberOfArticlesRelated && iterator < articleTagTuple.Count())
            {
                articlesRelated.Add(articleTagTuple.ElementAt(iterator).Item1);
                iterator++;
            }
            return articlesRelated;
        }
    }
}
