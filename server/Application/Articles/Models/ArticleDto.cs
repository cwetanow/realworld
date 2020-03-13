using System;
using System.Collections.Generic;
using System.Linq;
using Application.Common.Mappings;
using Application.Profiles.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Articles.Models
{
	public class ArticleDto : IMapFrom<Article>
	{
		public string Title { get; private set; }
		public string Description { get; private set; }
		public string Body { get; private set; }
		public string Slug { get; private set; }

		public IEnumerable<string> TagList { get; private set; } = new List<string>();

		public DateTime CreatedAt { get; private set; }
		public DateTime? UpdatedAt { get; private set; }

		public bool Favorited { get; set; }
		public int FavoritesCount { get; set; }

		public ProfileDto Author { get; set; }

		public void Mapping(Profile profile) => profile.CreateMap<Article, ArticleDto>()
				.ForMember(d => d.TagList,
					opt => opt.MapFrom(article => article.Tags.Select(t => t.Tag.Value)))
				.ForMember(d => d.Author,
					opt => opt.MapFrom(s => s.Author))
			;
	}
}
