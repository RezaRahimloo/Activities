using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Services.Auth;
using Application.Services.Auth.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : BaseApiController
    {
        private readonly IAuthService _auth;
        public AuthenticationController(IMediator mediator, 
                                        IAuthService auth,
                                        IHttpContextAccessor contextAccessor) : base(mediator, contextAccessor)
        {
            _auth = auth;
        }

        [HttpPost(nameof(Signin))]
        public async Task<ActionResult<UserDto>> Signin(SigninDto credentials)
        {

            // ModelState.AddModelError("email", "Email taken");
            // return BadRequest(ModelState);
            // return ValidationProblem();
            return HandleResult(await _auth.SigninUserAsync(credentials));
        }

        [HttpPost(nameof(Signup))]
        public async Task<ActionResult<TokenDto>> Signup(RegisterUserDto userDto)
        {
            return HandleResult(await _auth.RegisterUserAsync(userDto));
        }

        [HttpGet(nameof(GetCurrentUser))]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            return HandleResult(await _auth.GetCurrentUserAsync(getCurrentUserId()));
        }
    }
}