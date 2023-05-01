
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDto>>>
        {

        }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities
                    .Include(x => x.Attendess).ThenInclude(x => x.AppUser).ThenInclude(a => a.Photos)
                    .Select(x => new ActivityDto
                    {
                        Attendees = x.Attendess.Select(a => new AttendeeDto
                        {
                            Username = a.AppUser.Email,
                            DisplayName = a.AppUser.DisplayName,
                            Image = a.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url
                        }).ToList(),
                        Category = x.Category,
                        City = x.City,
                        Date = x.Date,
                        Description = x.Description,
                        HostUsername = x.Attendess.FirstOrDefault(a => a.IsHost).AppUser.Email,
                        IsCancelled = x.IsCancelled,
                        Title = x.Title,
                        Venue = x.Venue,
                        Id = x.Id

                    })
                    .ToListAsync(cancellationToken);

                var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(activitiesToReturn);
            }
        }
    }
}