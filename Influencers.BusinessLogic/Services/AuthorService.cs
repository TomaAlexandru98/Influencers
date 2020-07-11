using Influencers.BusinessLogic.ViewModels;
using Influencers.BusinessLogic.VIewModels;
using Influencers.Models;
using Influencers.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Influencers.BusinessLogic.Services
{
    public class AuthorService
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IArticleRepository articleRepository;

        public AuthorService(IAuthorRepository authorRepository, IArticleRepository articleRepository)
        {
            this.authorRepository = authorRepository;
            this.articleRepository = articleRepository;
        }

        public bool DoesAuthorExist(string email)
        {
            if (authorRepository.DoesAuthorExist(email)) return true;
            return false;
        }

        public Author Add(string email, string firstName, string lastName)
        {
            var authorToAdd = Author.Create(email, firstName, lastName);
            return authorRepository.Add(authorToAdd);
        }

        public IEnumerable<AuthorViewModel> GetAll()
        {
            var authorsList = new List<AuthorViewModel>();
            foreach (var author in authorRepository.GetAll())
            {
                var authorViewModel = new AuthorViewModel
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Votes = author.Votes,
                    ImageSource = author.ImageSource
                };
                authorsList.Add(authorViewModel);
            }
            return authorsList;
        }

        public Author GetById(int id)
        {
            return authorRepository.GetById(id);
        }

        public IEnumerable<string> GetAllAsString()
        {
            var authorsList = new List<string>();
            foreach (var author in authorRepository.GetAll())
            {
                authorsList.Add(author.FirstName + " " + author.LastName);
            }
            return authorsList;
        }

        public IEnumerable<AuthorViewModel> FilterByName(string searchAuthor)
        {
            var authorsList = new List<AuthorViewModel>();
            foreach (var author in authorRepository.FilterByName(searchAuthor))
            {
                var authorViewModel = new AuthorViewModel
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Votes = author.Votes,
                };
                authorsList.Add(authorViewModel);
            }
            return authorsList;
        }

        public DetailsAuthorViewModel GetAuthorAndArticles(int id)
        {
            var authorDb = authorRepository.GetById(id);
            var articlesDb = articleRepository.GetAllForAuthor(id);
            var articlesViewModelList = new List<ArticleViewModel>();
            foreach (var item in articlesDb)
            {
                var articleViewModel = new ArticleViewModel
                {
                    Id = item.Id,
                    Content = item.Content.Length < 200 ? item.Content : item.Content.Substring(0, 197) + "...",
                    Title = item.Title,
                    ImageSource = item.ImageSource,
                    Date = item.Date,
                };
                articlesViewModelList.Add(articleViewModel);
            };
            var detailsAuthorViewModel = new DetailsAuthorViewModel
            {
                Id = id,
                FirstName = authorDb.FirstName,
                LastName = authorDb.LastName,
                Votes = authorDb.Votes,
                Description = authorDb.Description,
                ImageSource = authorDb.ImageSource,
                Articles = articlesViewModelList
            };
            return detailsAuthorViewModel;
        }

        public Author Edit(int id, string description, IFormFile image, string imageDirectory)
        {
            var authorDb = authorRepository.GetById(id);
            if (image == null)
            {
                authorDb.Update(description);
            }
            else
            {
                var imageSource = GetImagePath(image, imageDirectory);
                authorDb.Update(description, imageSource);
            }
            return authorRepository.Update(authorDb);
        }

        private string GetImagePath(IFormFile image, string uploadDir)
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
    }
}
