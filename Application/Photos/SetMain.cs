using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.Email == _userAccessor.GetUsername());
                
                if(user is null)
                {
                    return null;
                }

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                if(photo is null)
                {
                    return null;
                }

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if(currentMain is null)
                {
                    return null;
                }

                photo.IsMain = true;
                currentMain.IsMain = false;

                var result = await _context.SaveChangesAsync() > 0;

                if(result)
                {
                    return Result<Unit>.Success(Unit.Value);
                }

                return Result<Unit>.Fail("Problem Setting Main Photo");
            }
        }
    }
}