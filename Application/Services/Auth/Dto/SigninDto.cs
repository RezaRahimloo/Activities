using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Auth.Dto
{
    public class SigninDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}