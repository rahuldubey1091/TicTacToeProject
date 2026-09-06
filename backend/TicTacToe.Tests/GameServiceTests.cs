using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private static GameService Service() => new();

    [Fact]
    public void NewGame_StartsWithX()
    {
        var game = Service().CreateGame(GameMode.TwoPlayer);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void ValidMove_SwitchesTurn()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new MoveRequest(Player.X, 0, 0));

        Assert.Null(game.Board[0, 1]);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);
    }

    [Fact]
    public void OccupiedMove_IsRejected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, new MoveRequest(Player.X, 0, 0));

        Assert.Throws<InvalidOperationException>(() =>
            service.MakeMove(game.Id, new MoveRequest(Player.O, 0, 0)));
    }

    [Fact]
    public void RowWin_IsDetected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 0));
        service.MakeMove(game.Id, new(Player.X, 0, 1));
        service.MakeMove(game.Id, new(Player.O, 1, 1));
        service.MakeMove(game.Id, new(Player.X, 0, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);
    }

    [Fact]
    public void ColumnWin_IsDetected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 0, 1));
        service.MakeMove(game.Id, new(Player.X, 1, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 1));
        service.MakeMove(game.Id, new(Player.X, 2, 0));

        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
    public void DiagonalWin_IsDetected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 0, 1));
        service.MakeMove(game.Id, new(Player.X, 1, 1));
        service.MakeMove(game.Id, new(Player.O, 1, 0));
        service.MakeMove(game.Id, new(Player.X, 2, 2));

        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
    public void Draw_IsDetected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        var moves = new[]
        {
            new MoveRequest(Player.X,0,0), new MoveRequest(Player.O,0,1),
            new MoveRequest(Player.X,0,2), new MoveRequest(Player.O,1,1),
            new MoveRequest(Player.X,1,0), new MoveRequest(Player.O,2,0),
            new MoveRequest(Player.X,1,2), new MoveRequest(Player.O,2,2),
            new MoveRequest(Player.X,2,1)
        };

        foreach (var move in moves) service.MakeMove(game.Id, move);

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
    }

    [Fact]
    public void Undo_TwoPlayer_RemovesOneMoveAndRestoresTurn()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 1));

        service.Undo(game.Id);

        Assert.Null(game.Board[1,1]);
        Assert.Single(game.Moves);
        Assert.Equal(Player.O, game.CurrentPlayer);
    }

    [Fact]
    public void ComputerMode_MakesAutomaticOResponse()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.Computer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));

        Assert.Equal(2, game.Moves.Count);
        Assert.Equal(Player.X, game.Moves[0].Player);
        Assert.Equal(Player.O, game.Moves[1].Player);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public void ComputerMode_Undo_RemovesHumanAndComputerMoves()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.Computer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.Undo(game.Id);

        Assert.Empty(game.Moves);
        Assert.Null(game.Board[0,0]);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public void Scoreboard_IncrementsOnlyOnce()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 0));
        service.MakeMove(game.Id, new(Player.X, 0, 1));
        service.MakeMove(game.Id, new(Player.O, 1, 1));
        service.MakeMove(game.Id, new(Player.X, 0, 2));

        Assert.Equal(1, service.GetScoreboard().XWins);
    }

    [Fact]
    public void MoveAfterCompletion_IsRejected()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 0));
        service.MakeMove(game.Id, new(Player.X, 0, 1));
        service.MakeMove(game.Id, new(Player.O, 1, 1));
        service.MakeMove(game.Id, new(Player.X, 0, 2));

        Assert.Throws<InvalidOperationException>(() =>
            service.MakeMove(game.Id, new(Player.O, 2, 2)));
    }

    [Fact]
    public void ResetGame_ClearsBoardButKeepsScoreboard()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, new(Player.X, 0, 0));
        service.MakeMove(game.Id, new(Player.O, 1, 0));
        service.MakeMove(game.Id, new(Player.X, 0, 1));
        service.MakeMove(game.Id, new(Player.O, 1, 1));
        service.MakeMove(game.Id, new(Player.X, 0, 2));

        var reset = service.ResetGame(game.Id);

        Assert.Empty(reset.Moves);
        Assert.Equal(Player.X, reset.CurrentPlayer);
        Assert.Equal(1, service.GetScoreboard().XWins);
    }

    [Fact]
    public void Computer_BlocksImmediateXWin()
    {
        var service = Service();
        var game = service.CreateGame(GameMode.Computer);

        // X at (0,0), O automatically center.
        service.MakeMove(game.Id, new(Player.X, 0, 0));
        // X at (0,1), O should block (0,2).
        service.MakeMove(game.Id, new(Player.X, 0, 1));

        Assert.Equal(Player.O, game.Board[0,2]);
    }
}
