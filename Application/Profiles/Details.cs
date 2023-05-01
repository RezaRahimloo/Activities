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

namespace Application.Profiles
{
    public class Details
    {
        public class Query : IRequest<Result<Profile>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Profile>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(u => u.Photos)
                .Select(u => new Profile
                {
                    Bio = u.Bio,
                    DisplayName = u.FirstName,
                    Username = u.Email,
                    Image = u.Photos.FirstOrDefault(p => p.IsMain).Url,
                    Photos = u.Photos
                }).SingleOrDefaultAsync(x => x.Username == request.Username);

                if(user is null)
                {
                    return null;
                }

                return Result<Profile>.Success(user);

            }
        }
    }
}