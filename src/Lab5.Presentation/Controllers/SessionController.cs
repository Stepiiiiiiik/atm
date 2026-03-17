using Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;
using Lab5.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Controllers;

[Route("sessions")]
public class SessionController : Controller
{
    private const string AdminPassword = "sa";

    private readonly IAccountService _accountService;

    private readonly ISessionService _sessionService;

    public SessionController(
        IAccountService accountService,
        ISessionService sessionService)
    {
        _accountService = accountService;
        _sessionService = sessionService;
    }

    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdminSession([FromBody] CreateAdminSessionRequest request)
    {
        if (request.AdminPassword != AdminPassword)
        {
            return Unauthorized("Wrong admin password");
        }

        OperationResult<Guid> result = await _sessionService.CreateAdminSession();

        return result switch
        {
            OperationResult<Guid>.Success success => Ok(new { SessionKey = success.Result }),
            OperationResult<Guid>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<Guid>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<Guid>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUserSession([FromBody] CreateUserSessionRequest request)
    {
        if (request == null)
        {
            return BadRequest("NULL NULL NULL");
        }

        if (!Guid.TryParse(request.AccountId, out Guid accountId))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<bool> checkResult = await _accountService.CheckAccountData(request.Pin, accountId);

        if (checkResult is not OperationResult<bool>.Success)
        {
            return Unauthorized("Invalid account or PIN");
        }

        OperationResult<Guid> result = await _sessionService.CreateUserSession(accountId);

        return result switch
        {
            OperationResult<Guid>.Success success => Ok(new { SessionKey = success.Result }),
            OperationResult<Guid>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<Guid>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<Guid>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }
}