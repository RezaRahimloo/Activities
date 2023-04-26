
using System;
using System.Text;
using Application.Services.Auth.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Persistance;

namespace API.Controllers
{
    
    public class BuggyController : BaseApiController
    {
        private readonly DataContext context;

        public BuggyController(IMediator mediator, DataContext context) : base(mediator)
        {
            this.context = context;
        }
        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            return BadRequest("This is a bad request");
        }

        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {
            throw new Exception("This is a server error");
        }

        [HttpGet("unauthorised")]
        public ActionResult GetUnauthorised()
        {
            return Unauthorized();
        }

        [HttpGet("SeedUsers")]
        public async Task<ActionResult> SeedUsers()
        {
            if(!context.Users.Any())
            {
                var users = new List<RegisterUserDto>
                {
                    new RegisterUserDto
                    {
                        DisplayName = "Bob",
                        Email = "bob@test.com",
                        FirstName = "Bob",
                        LastName = "Marlin",
                        Password = "Password1",
                        ConfirmPassword = "Password1",
                        Bio = ""
                        
                    },
                    new RegisterUserDto
                    {
                        DisplayName = "Tom",
                        Email = "tom@test.com",
                        FirstName = "Tom",
                        LastName = "Cruise",
                        Password = "Password1",
                        ConfirmPassword = "Password1",
                        Bio = ""
                    },
                    new RegisterUserDto
                    {
                        DisplayName = "Jane",
                        Email = "jane@test.com",
                        FirstName = "Jane",
                        LastName = "Jade",
                        Password = "Password1",
                        ConfirmPassword = "Password1",
                        Bio = ""
                    }
                };
                using(var client = new HttpClient())
                {
                    var tasks = new List<Task<HttpResponseMessage>>();
                    string url = "https://localhost:5001/api/Authentication/Signup";
                    foreach(var user in users)
                    {
                        var json = JsonConvert.SerializeObject(user);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        
                        tasks.Add(client.PostAsync(url, data));
                    }
                    Task.WaitAll(tasks.ToArray());

                    foreach(var task in tasks)
                    {
                        var data = task.Result;
                        Console.WriteLine(data.Content.ToString());
                    }
                }
            }
            return Ok();
        }
    }
}