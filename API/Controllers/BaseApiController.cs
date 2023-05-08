using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        private IHttpContextAccessor _contextAccessor;
        protected IMediator Mediator
        {
            get
            {
                return _mediator ??= HttpContext
                                        .RequestServices
                                        .GetService<IMediator>();
            }
        }
        protected IHttpContextAccessor ContextAccessor => _contextAccessor ??= HttpContext
                                        .RequestServices
                                        .GetService<IHttpContextAccessor>();

        public BaseApiController(IMediator mediator, IHttpContextAccessor contextAccessor)
        {
            _mediator = mediator;
            _contextAccessor = contextAccessor;
        }

        protected string getCurrentUserId()
        {
            return _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        protected ActionResult HandleResult<T>(Result<T>? result)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            if(result is null)
            {
                return NotFound();
            }
            if(result.IsSuccess && result.Value != null)
            {
                return Ok(result.Value);
            }
            if(result.IsSuccess && result.Value == null)
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        protected ActionResult HandlePagedResult<T>(Result<PagedList<T>>? result)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            if(result is null)
            {
                return NotFound();
            }
            if(result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.CurrentPage, 
                    result.Value.PageSize, 
                    result.Value.TotalCount, 
                    result.Value.TotalPages);
                
                return Ok(result.Value);
            }
            if(result.IsSuccess && result.Value == null)
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        
    }
}