using Influencers.Models;
using Influencers.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Influencers.BusinessLogic.Services
{
    public class TagService
    {
        private readonly ITagRepository tagRepository;
        private readonly IArticleRepository articleRepository;

        public TagService(ITagRepository tagRepository, IArticleRepository articleRepository)
        {
            this.tagRepository = tagRepository;
            this.articleRepository = articleRepository;
        }

        public IEnumerable<string> GetAllToString()
        {
            var tagsList = new List<string>();
            foreach (var tag in tagRepository.GetAll())
            {
                tagsList.Add(tag.Name);
            }
            return tagsList.ToList();
        }

        public Tag Add(string name)
        {
            var tagToAdd = Tag.Create(name);
            return tagRepository.Add(tagToAdd);
        }

        public IEnumerable<Tag> AddMultipleTags(string tags)
        {
            var tagsToAdd = new List<Tag>();
            foreach (var tagString in ConvertStringToList(tags))
            {
                if (tagRepository.DoesTagExists(tagString) == false)
                {
                    var tagToAdd = Tag.Create(tagString);
                    tagsToAdd.Add(tagToAdd);
                }
            }
            return tagRepository.AddMultipleTags(tagsToAdd);
        }

        public IEnumerable<Tag> GetForArticleFromString(string tags)
        {
            var tagsList = new List<Tag>();
            foreach (var tag in ConvertStringToList(tags))
            {
                var tagDb = tagRepository.GetByName(tag);
                tagsList.Add(tagDb);
            }
            return tagsList;
        }

        public string GetNamesAsAString(int id)
        {
            string tags = "";
            var i = 0;
            var articleTags = articleRepository.GetArticleTags(id).ToList();
            foreach (var articleTag in articleTags)
            {
                if (articleTags.Count() == i + 1)
                {
                    tags += articleTag.Tag.Name;
                    return tags;
                }
                tags += (articleTag.Tag.Name + ",");
                i++;
            }
            return tags;
        }

        private IEnumerable<string> ConvertStringToList(string stringToConvert)
        {
            var stringsList = new List<string>();
            while (stringToConvert != "")
            {
                var indexOfComma = stringToConvert.IndexOf(",");
                if (indexOfComma != -1)
                {
                    var substring = stringToConvert.Substring(0, indexOfComma);
                    stringsList.Add(substring);
                    stringToConvert = stringToConvert.Remove(0, indexOfComma + 1);
                }
                else
                {
                    stringsList.Add(stringToConvert);
                    stringToConvert = "";
                }
            }
            return stringsList;
        }

        
    }
}
