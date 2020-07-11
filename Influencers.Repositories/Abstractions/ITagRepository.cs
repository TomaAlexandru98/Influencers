using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.Repositories.Abstractions
{
    public interface ITagRepository : IBaseRepository<Tag>
    {
        bool DoesTagExists(string tagName);
        IEnumerable<Tag> AddMultipleTags(List<Tag> tagsToAdd);
        Tag GetByName(string tag);
    }
}
