using System.Threading;
using System.Threading.Tasks;
using Application.Tags.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tags.Queries
{
    public class TagListQuery : IRequest<TagListDto>
    {
        public class Handler : IRequestHandler<TagListQuery, TagListDto>
        {
            private readonly DbContext context;
            private readonly IMapper mapper;

            public Handler(DbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<TagListDto> Handle(TagListQuery request, CancellationToken cancellationToken)
            {
                var tags = await context.Set<Tag>()
                    .ToListAsync(cancellationToken);

                return mapper.Map<TagListDto>(tags);
            }
        }
    }
}