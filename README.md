# Tic Tac Toe 

Full-stack Tic Tac Toe application built for the Round 2 problem statement.

## Stack

- Frontend: Angular 18 + TypeScript
- Backend: .NET 8 Web API
- API: REST
- Storage: In-memory
- Tests: xUnit

## Project Structure

```text
tictactoe/
├── frontend/
└── backend/
    ├── TicTacToe.Api/
    └── TicTacToe.Tests/
```

## Backend

```bash
cd backend
dotnet restore
dotnet run --project TicTacToe.Api
```

API runs on `http://localhost:5000`.

Swagger:
`http://localhost:5000/swagger`

Run tests:

```bash
dotnet test
```

## Frontend

Prerequisites: Node.js 18+ and Angular CLI.

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200`.

## API

- `POST /api/games` - create a game
- `GET /api/games/{id}` - get current game
- `POST /api/games/{id}/moves` - submit a move
- `POST /api/games/{id}/undo` - undo
- `POST /api/games/{id}/reset` - reset game
- `GET /api/scoreboard` - get scoreboard
- `POST /api/scoreboard/reset` - reset scoreboard

Create game request:

```json
{ "mode": "TwoPlayer" }
```

or

```json
{ "mode": "Computer" }
```

Move request:

```json
{
  "player": "X",
  "row": 0,
  "column": 0
}
```

## Computer Strategy

The computer is O and follows the required priority:

1. Win if O can win.
2. Block X if X can win next.
3. Take center.
4. Take a corner.
5. Take any available cell.

## Undo Decision

Option A is used: Undo is disabled after a game is completed. This keeps a completed scoreboard result immutable.

In Computer mode, one undo removes the human X move and the computer O response together.

## Design

The backend is the source of truth. Game rules, validation, move history, game status and scoreboard are maintained by the backend. Angular renders the latest API response.

## AI-Assisted Development

AI was used to accelerate scaffolding, implementation and test generation. The generated code was reviewed and adapted for the requirements, especially game state transitions, undo behavior, scoreboard consistency and computer move priority.

## Known Limitations

- Storage is in-memory and is lost when the API restarts.
- No authentication is required.
- A production deployment would normally use persistent storage and stronger concurrency controls.

## Future Improvements

- SQLite persistence
- Authentication and multiple users
- Persistent game history
- Smarter AI
- Integration/E2E tests
