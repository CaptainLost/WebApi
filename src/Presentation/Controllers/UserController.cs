using Application.Abstractions.Messaging.Commands;
using Application.Users.Login;
using Domain.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class UserController(ICommandDispatcher commandDispatcher) : BaseController
{
    private readonly ICommandDispatcher m_commandDispatcher = commandDispatcher;

    [HttpPost("login")]
    public async Task<ActionResult<Result>> Login([FromBody] LoginCommand command)
    {
        Result result = await m_commandDispatcher.Dispatch(command);

        return Ok(result);
    }
}
