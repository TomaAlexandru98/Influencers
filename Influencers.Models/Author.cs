using System;
using System.Collections.Generic;

namespace Influencers.Models
{
    public partial class Author : DataEntity
    {
        public Author()
        {
            Article = new HashSet<Article>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Votes { get; set; }
        public string ImageSource { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Article> Article { get; set; }

        public static Author Create(string email, string lastName, string firstName)
        {
            return new Author
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Votes = 0
            };
        }

        public int? GetVote(int value)
        {
            this.Votes += value;
            return this.Votes;
        }

        public Author Update(string description, string imageSource)
        {
            this.Description = description;
            this.ImageSource = imageSource;
            return this;
        }

        public Author Update(string description)
        {
            this.Description = description;
            return this;
        }
    }
}
