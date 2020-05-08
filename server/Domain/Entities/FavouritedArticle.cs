using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class FavouritedArticle : ValueObject
    {
        private FavouritedArticle()
        {
        }

        public FavouritedArticle(int articleId, int userId)
        {
            ArticleId = articleId;
            UserId = userId;
        }

        public int ArticleId { get; }
        public Article Article { get; }

        public int UserId { get; }
        public UserProfile User { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ArticleId;
            yield return UserId;
        }
    }
}