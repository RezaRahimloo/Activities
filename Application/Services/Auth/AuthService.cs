using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistance;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Application.Core;
using Application.Services.Auth.Dto;
using System.Text.RegularExpressions;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthService(DataContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public async Task<Result<TokenDto>> SigninUserAsync(SigninDto credentials)
        {
            if(string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return Result<TokenDto>.Fail("Signin information is not valid!");
            }
            if(!validatePassword(credentials.Password))
            {
                return Result<TokenDto>.Fail("User not found!");
            }
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == credentials.Email);
            if(user is null)
            {
                return Result<TokenDto>.Fail("User not found!");
            }

            if(!verifyPasswordHash(credentials.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Result<TokenDto>.Fail("User not found!");
            }

            return Result<TokenDto>.Success(new TokenDto { JWTToken = createToken(user) });
        }

        public async Task<Result<TokenDto>> RegisterUserAsync(RegisterUserDto user)
        {
            bool isEmailValid = Regex.IsMatch(user.Email, @"^([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$");
            if(!isEmailValid)
            {
                return Result<TokenDto>.Fail("Entered email is not a valid email address!");
            }
            string? passwordError = validatePassword(user.Password, user.ConfirmPassword);
            if(passwordError is not null)
            {
                return Result<TokenDto>.Fail(passwordError); 
            }
            if(await userExistsAsync(user.Email))
            {
                return Result<TokenDto>.Fail($"A user has already created an account with this email: {user.Email}");
            }
            createPasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var appUser = new AppUser
            {
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash

            };
            _context.Users.Add(appUser);
            bool didAdd = await _context.SaveChangesAsync() > 0;

            if(!didAdd)
            {
                throw new Exception($"no database row was affected while adding a user at: {DateTime.Now}");
            }

            string token = createToken(appUser);
            return Result<TokenDto>.Success(new TokenDto { JWTToken = token });
        }

        private string? validatePassword(string password, string confirmPassword)
        {
            bool isPasswordValid = validatePassword(password);
            if(!isPasswordValid)
            {
                return "Password must contain at least characters.\nPassword must contain at least one uppercase letter.\nPassword must contain at least one lowercase letter.\nPasswrod must contain at least one number.";
            }

            if(password != confirmPassword)
            {
                return "Passwords don't match";
            }

            return null;
        }
        private bool validatePassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*?]{8,}$");
        }

        private async Task<bool> userExistsAsync(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email cannot be empty");
            }
            if(await _context.Users.AnyAsync(user => user.Email == email.ToLower() && user.isEmailConfirmed))
            {
                return true;
            }
            return false;
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // compute the entered password's hash
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                //Check if the password hash byte array is equal to the password hash saved in the DB
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string createToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:JWTToken").Value ?? ""));

            Console.WriteLine(_config.GetSection("AppSettings:JWTToken").Value);

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(3),
                SigningCredentials = credentials,

            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}