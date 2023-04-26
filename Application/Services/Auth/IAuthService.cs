using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Services.Auth.Dto;

namespace Application.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<TokenDto>> SigninUserAsync(SigninDto credentials);
        Task<Result<TokenDto>> RegisterUserAsync(RegisterUserDto user);
    }
}