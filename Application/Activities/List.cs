
using Application.Core;
using Application.Interfaces;
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
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Activities
                    .Include(x => x.Attendess).ThenInclude(x => x.AppUser).ThenInclude(a => a.Photos)
                    .Where(d => d.Date >= request.Params.StartDate)
                    .OrderBy(d => d.Date)
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
                    //.ToListAsync(cancellationToken);
                    .AsQueryable();
                
                if(request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if(request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                var activitiesToReturn = //_mapper.Map<List<ActivityDto>>(activities);
                    await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<ActivityDto>>.Success(activitiesToReturn);
            }
        }
    }
}