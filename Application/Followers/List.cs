using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string Predicate { get; set; }
            public string Username { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profiles = new List<Profiles.Profile>();

                switch(request.Predicate)
                {
                    case "followers":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Target.Email == request.Username)
                            .Include(x => x.Target).ThenInclude(x => x.Photos)
                            .Include(x => x.Target).ThenInclude(x => x.Followers)
                            .Select(u => u.Observer)
                            .Select(u => new Profiles.Profile
                            {
                                Bio = u.Bio,
                                DisplayName = u.DisplayName,
                                FollowersCount = u.Followers.Count(),
                                FollowingCount = u.Followings.Count(),
                                Image = u.Photos.FirstOrDefault(i => i.IsMain).Url,
                                Username = u.Email,
                                Photos = u.Photos,
                                Following = u.Followers.Any(x => x.Observer.Email == _userAccessor.GetUsername())
                            }).ToListAsync();
                        break;
                    case "following":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Observer.Email == request.Username)
                            .Include(x => x.Target).ThenInclude(x => x.Photos)
                            .Include(x => x.Target).ThenInclude(x => x.Followers).ThenInclude(x => x.Observer)
                            .Select(u => u.Target)
                            .Select(u => new Profiles.Profile
                            {
                                Bio = u.Bio,
                                DisplayName = u.DisplayName,
                                FollowersCount = u.Followers.Count,
                                FollowingCount = u.Followings.Count,
                                Image = u.Photos.FirstOrDefault(i => i.IsMain).Url,
                                Username = u.Email,
                                Photos = u.Photos,
                                Following = u.Followers.Any(x => x.Target.Email == _userAccessor.GetUsername())
                            }).ToListAsync();
                        break;
                }

                return Result<List<Profiles.Profile>>.Success(profiles);

            }
        }
    }
}