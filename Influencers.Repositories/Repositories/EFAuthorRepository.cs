using Influencers.Models;
using Influencers.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Influencers.Repositories.Repositories
{
    public class EFAuthorRepository : EFBaseRepository<Author>, IAuthorRepository
    {
        public EFAuthorRepository(BloggingContext dbContext) : base(dbContext)
        {
        }

        public bool DoesAuthorExist(string email)
        {
            var authorDb = GetByEmail(email);
            if (authorDb != null) return true;
            return false;
        }

        public IEnumerable<Author> FilterByName(string searchAuthor)
        {
            return dbContext.Author.Where(author => (author.FirstName.Contains(searchAuthor)) ||
                                                     (author.LastName.Contains(searchAuthor)));
        }

        public Author GetByEmail(string email)
        {
            return dbContext.Author.Where(author => author.Email == email)
                                   .SingleOrDefault();
        }
    }
}
