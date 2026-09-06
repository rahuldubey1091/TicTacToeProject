using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController(GameService service) : ControllerBase
{
    [HttpPost]
    public ActionResult<GameResponse> Create(CreateGameRequest request)
    {
        var game = service.CreateGame(request.Mode);
        return Ok(GameService.ToResponse(game, service.GetScoreboard()));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GameResponse> Get(Guid id)
    {
        var game = service.GetGame(id);
        if (game is null) return NotFound(new ApiError { Error = "Game not found." });
        return Ok(GameService.ToResponse(game, service.GetScoreboard()));
    }

    [HttpPost("{id:guid}/moves")]
    public ActionResult<GameResponse> Move(Guid id, MoveRequest request)
    {
        try
        {
            var game = service.MakeMove(id, request);
            return Ok(GameService.ToResponse(game, service.GetScoreboard()));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiError { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiError { Error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/undo")]
    public ActionResult<GameResponse> Undo(Guid id)
    {
        try
        {
            var game = service.Undo(id);
            return Ok(GameService.ToResponse(game, service.GetScoreboard()));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiError { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiError { Error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reset")]
    public ActionResult<GameResponse> Reset(Guid id)
    {
        try
        {
            var game = service.ResetGame(id);
            return Ok(GameService.ToResponse(game, service.GetScoreboard()));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiError { Error = ex.Message });
        }
    }
}
