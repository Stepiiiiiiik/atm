using Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;
using Lab5.Core.Abstractions;
using Lab5.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Controllers;

[Route("accounts")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    private readonly ILoginService _loginService;

    public AccountController(IAccountService accountService, ILoginService loginService)
    {
        _accountService = accountService;
        _loginService = loginService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        if (!Guid.TryParse(request.SessionKey, out Guid sessionGuid))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<bool> isAdmin = await _loginService.LoginAdmin(sessionGuid);

        if (isAdmin is not OperationResult<bool>.Success or OperationResult<bool>.Success { Result: false })
        {
            return Unauthorized("Access denied. Invalid key or you have no rights");
        }

        OperationResult<Guid> result = await _accountService.CreateAccount(request.Pin, sessionGuid);

        return result switch
        {
            OperationResult<Guid>.Success success => Ok(new { AccountId = success.Result }),
            OperationResult<Guid>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<Guid>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<Guid>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }

    [HttpPost("balance")]
    public async Task<IActionResult> CheckBalance([FromBody] AccountBasicOperationsRequest request)
    {
        if (!Guid.TryParse(request.SessionKey, out Guid sessionGuid))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<Guid> isUser = await _loginService.LoginUser(sessionGuid);

        if (isUser is not OperationResult<Guid>.Success loginSuccess)
        {
            return Unauthorized("Access denied. Invalid key or you have no rights");
        }

        Guid accountId = loginSuccess.Result;

        OperationResult<decimal> result = await _accountService.CheckBalance(accountId, sessionGuid);

        return result switch
        {
            OperationResult<decimal>.Success success => Ok(new { Balance = success.Result }),
            OperationResult<decimal>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<decimal>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<decimal>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] AccountDepositRequest request)
    {
        if (!Guid.TryParse(request.SessionKey, out Guid sessionGuid))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<Guid> isUser = await _loginService.LoginUser(sessionGuid);

        if (isUser is not OperationResult<Guid>.Success loginSuccess)
        {
            return Unauthorized("Access denied. Invalid key or you have no rights");
        }

        Guid accountId = loginSuccess.Result;

        OperationResult<decimal> result = await _accountService.Deposit(accountId, request.Amount, sessionGuid);

        return result switch
        {
            OperationResult<decimal>.Success success => Ok(new { Amount = success.Result }),
            OperationResult<decimal>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<decimal>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<decimal>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] AccountWithdrawRequest request)
    {
        if (!Guid.TryParse(request.SessionKey, out Guid sessionGuid))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<Guid> isUser = await _loginService.LoginUser(sessionGuid);

        if (isUser is not OperationResult<Guid>.Success loginSuccess)
        {
            return Unauthorized("Access denied. Invalid key or you have no rights");
        }

        Guid accountId = loginSuccess.Result;

        OperationResult<decimal> result = await _accountService.Withdraw(accountId, request.Amount, sessionGuid);

        return result switch
        {
            OperationResult<decimal>.Success success => Ok(new { Amount = success.Result }),
            OperationResult<decimal>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<decimal>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<decimal>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromHeader(Name = "sessionKey")] string sessionKey)
    {
        if (!Guid.TryParse(sessionKey, out Guid sessionGuid))
            return BadRequest("SessionKey must be a valid GUID format");

        OperationResult<Guid> isUser = await _loginService.LoginUser(sessionGuid);

        if (isUser is not OperationResult<Guid>.Success loginSuccess)
        {
            return Unauthorized("Access denied. Invalid key or you have no rights");
        }

        Guid accountId = loginSuccess.Result;

        OperationResult<IReadOnlyCollection<HistoryItem>> result =
            await _accountService.GetHistory(accountId, sessionGuid);

        return result switch
        {
            OperationResult<IReadOnlyCollection<HistoryItem>>.Success history => Ok(history.Result.Select(HistoryItemDto.CreateFrom)),
            OperationResult<IReadOnlyCollection<HistoryItem>>.BadRequest badRequest => BadRequest(badRequest.Message),
            OperationResult<IReadOnlyCollection<HistoryItem>>.Unauthorized unauthorized => Unauthorized(unauthorized.Message),
            OperationResult<IReadOnlyCollection<HistoryItem>>.InternalError internalError => StatusCode(500, internalError.Message),
            _ => StatusCode(500, "Unknown error"),
        };
    }
}