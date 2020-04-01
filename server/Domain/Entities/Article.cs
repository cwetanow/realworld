using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;

namespace Domain.Entities
{
    public class Article : Entity
    {
        private Article()
        {
        }

        public Article(string title, string description, string body, DateTime created, UserProfile author,
            IEnumerable<Tag> tags = null)
        {
            Title = title;
            Description = description;
            Body = body;
            CreatedAt = created;
            UpdatedAt = created;
            Author = author;

            SetTags(tags);
            SetSlug();
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Body { get; private set; }

        public string Slug { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public ICollection<ArticleTag> Tags { get; private set; } = new List<ArticleTag>();

        public int AuthorId { get; private set; }
        public UserProfile Author { get; private set; }

        public void Update(string newTitle, string newDescription, string newBody)
        {
            if (!string.IsNullOrEmpty(newTitle) && newTitle != this.Title)
            {
                this.Title = newTitle;
                this.SetSlug();
            }

            if (!string.IsNullOrEmpty(newDescription) && newDescription != this.Description)
            {
                this.Description = newDescription;
            }

            if (!string.IsNullOrEmpty(newBody) && newBody != this.Body)
            {
                this.Body = newBody;
            }
        }

        private void SetSlug() => Slug = string.Join('-', Title.ToLower().Split(' '));

        private void SetTags(IEnumerable<Tag> tags = null)
        {
            tags ??= new List<Tag>();

            Tags = tags
                .Select(t => new ArticleTag(t, this))
                .ToList();
        }
    }
}