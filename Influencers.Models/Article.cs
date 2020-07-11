using System;
using System.Collections.Generic;

namespace Influencers.Models
{
    public partial class Article : DataEntity
    {
        public Article()
        {
            ArticleTag = new HashSet<ArticleTag>();
        }

        public int? AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public int? Views { get; set; }
        public string ImageSource { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<ArticleTag> ArticleTag { get; set; }

        public static Article Create(string title, string content, string imageSource, Author author)
        {
            return new Article
            {
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Author = author,
                Date = DateTime.UtcNow,
                Views = 0
            };
        }

        public Article Update(string title, string content)
        {
            this.Title = title;
            this.Content = content;
            return this;
        }

        public double GetTrendingCoefficient()
        {
            return DateTime.Now.Subtract(this.Date.Value).TotalMinutes / Views.Value;
        }

        public int? IncreaseViews()
        {
            this.Views++;
            return this.Views;
        }
    }
}
