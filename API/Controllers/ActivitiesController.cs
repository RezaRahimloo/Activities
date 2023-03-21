using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;


namespace API.Controllers
{

    public class ActivitiesController : BaseApiController
    {
        public ActivitiesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new List.Query());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity?>> GetActivity(Guid id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            await Mediator.Send(new Create.Command { Activity = activity });
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid Id,Activity activity)
        {
            activity.Id = Id;
            await Mediator.Send(new Edit.Command { Activity = activity });
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid Id)
        {
            await Mediator.Send(new Delete.Command { Id = Id });

            return Ok();
        }
    }
}