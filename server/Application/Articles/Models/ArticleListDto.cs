using System.Collections.Generic;
using System.Linq;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Articles.Models
{
	public class ArticleListDto : IMapFrom<IEnumerable<Article>>
	{
		public IEnumerable<ArticleDto> Articles { get; set; } = new List<ArticleDto>();

		public int ArticlesCount => Articles.Count();

		public void Mapping(Profile profile) => profile.CreateMap<IEnumerable<ArticleDto>, ArticleListDto>()
			.ForMember(d => d.Articles, opt => opt.MapFrom(s => s));
	}
}
