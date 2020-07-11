using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.Repositories.Abstractions
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        bool DoesAuthorExist(string email);
        Author GetByEmail(string email);
        IEnumerable<Author> FilterByName(string searchAuthor);
    }
}
