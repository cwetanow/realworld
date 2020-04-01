using System.Collections.Generic;
using System.Linq;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Tags.Models
{
    public class TagListDto : IMapFrom<IEnumerable<Tag>>
    {
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        public void Mapping(Profile profile) => profile.CreateMap<IEnumerable<Tag>, TagListDto>()
            .ForMember(d => d.Tags, opts => opts.MapFrom(s => s.Select(t => t.Value)));
    }
}