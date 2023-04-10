using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        protected IMediator Mediator
        {
            get
            {
                return _mediator ??= HttpContext
                                        .RequestServices
                                        .GetService<IMediator>();
            }
        }

        public BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected ActionResult HandleResult<T>(Result<T>? result)
        {
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
    }
}