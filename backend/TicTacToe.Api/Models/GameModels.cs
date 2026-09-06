namespace TicTacToe.Api.Models;

public enum Player
{
    X,
    O
}

public enum GameMode
{
    TwoPlayer,
    Computer
}

public enum GameStatus
{
    InProgress,
    Won,
    Draw
}

public sealed class Move
{
    public int MoveNumber { get; init; }
    public Player Player { get; init; }
    public int Row { get; init; }
    public int Column { get; init; }
}

public sealed class Game
{
    public Guid Id { get; init; }

    // IMPORTANT:
    // Keep this as a 2D array internally.
    public Player?[,] Board { get; } = new Player?[3, 3];

    public Player CurrentPlayer { get; set; } = Player.X;

    public GameMode Mode { get; init; }

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public Player? Winner { get; set; }

    public List<int[]> WinningCells { get; set; } = [];

    public List<Move> Moves { get; } = [];

    public bool ScoreApplied { get; set; }
}

public sealed record CreateGameRequest(GameMode Mode);

public sealed record MoveRequest(
    Player Player,
    int Row,
    int Column);

public sealed class Scoreboard
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
}

public sealed class GameResponse
{
    public Guid GameId { get; init; }

    // IMPORTANT:
    // Response uses jagged array because System.Text.Json
    // serializes this correctly.
    public string?[][] Board { get; init; } =
    [
        new string?[3],
        new string?[3],
        new string?[3]
    ];

    public Player CurrentPlayer { get; init; }

    public GameMode GameMode { get; init; }

    public GameStatus GameStatus { get; init; }

    public Player? Winner { get; init; }

    public List<int[]> WinningCells { get; init; } = [];

    public List<Move> MoveHistory { get; init; } = [];

    public Scoreboard Scoreboard { get; init; } = new();
}

public sealed class ApiError
{
    public string Error { get; init; } = "";
}