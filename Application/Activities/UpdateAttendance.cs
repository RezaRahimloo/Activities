using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _accessor;

            public Handler(DataContext context, IUserAccessor accessor)
            {
                _context = context;
                _accessor = accessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(a => a.Attendess).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if(activity is null)
                {
                    return null;
                }
                string email = _accessor.GetUsername();
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == _accessor.GetUsername());
                
                if(user is null)
                {
                    return null;
                }

                var hostUsername = activity.Attendess.FirstOrDefault(x => x.IsHost)?.AppUser?.Email;

                var attendance = activity.Attendess.FirstOrDefault(x => x.AppUser.Email == user.Email);

                if(attendance != null && hostUsername == user.Email)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }
                if(attendance != null && hostUsername != user.Email)
                {
                    activity.Attendess.Remove(attendance);
                }
                if(attendance is null)
                {
                    attendance = new ActivityAttendee
                    {
                        AppUser = user,
                        Activity = activity,
                        IsHost = false
                    };

                    activity.Attendess.Add(attendance);
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Fail("Problem updating attendance");
            }
        }
    }
}