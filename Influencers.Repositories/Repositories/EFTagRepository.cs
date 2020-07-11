using Influencers.Models;
using Influencers.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Influencers.Repositories.Repositories
{
    public class EFTagRepository : EFBaseRepository<Tag>, ITagRepository
    {
        public EFTagRepository(BloggingContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Tag> AddMultipleTags(List<Tag> tags)
        {
            dbContext.Tag.AddRange(tags);
            dbContext.SaveChanges();
            return tags;
        }

        public bool DoesTagExists(string tagName)
        {
            var tagDb = dbContext.Tag.Where(tag => tag.Name == tagName).SingleOrDefault();
            if (tagDb == null) return false;
            return true;
        }

        public Tag GetByName(string name)
        {
            return dbContext.Tag.Where(tag => tag.Name == name).SingleOrDefault();
        }
    }
}
