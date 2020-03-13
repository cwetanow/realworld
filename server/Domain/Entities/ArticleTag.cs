namespace Domain.Entities
{
	public class ArticleTag
	{
		public ArticleTag(int tagId, int articleId)
		{
			TagId = tagId;
			ArticleId = articleId;
		}

		public ArticleTag(Tag tag, Article article)
		{
			Tag = tag;
			Article = article;
		}

		public int TagId { get; }
		public Tag Tag { get; }

		public int ArticleId { get; }
		public Article Article { get; }
	}
}
