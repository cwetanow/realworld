using Domain.Common;

namespace Domain.Entities
{
    public class FavouritedArticle : Entity
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
    }
}