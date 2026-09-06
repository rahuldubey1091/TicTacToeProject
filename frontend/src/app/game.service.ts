import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export type Player = 'X' | 'O';
export type GameMode = 'TwoPlayer' | 'Computer';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';

export interface Move {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface GameState {
  gameId: string;
  board: (Player | null)[][];
  currentPlayer: Player;
  gameMode: GameMode;
  gameStatus: GameStatus;
  winner: Player | null;
  winningCells: number[][];
  moveHistory: Move[];
  scoreboard: Scoreboard;
}

@Injectable({ providedIn: 'root' })
export class GameService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  createGame(mode: GameMode): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games`, { mode });
  }

  getGame(id: string): Observable<GameState> {
    return this.http.get<GameState>(`${this.baseUrl}/games/${id}`);
  }

  move(id: string, player: Player, row: number, column: number): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${id}/moves`, { player, row, column });
  }

  undo(id: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${id}/undo`, {});
  }

  resetGame(id: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${id}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.baseUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.baseUrl}/scoreboard/reset`, {});
  }
}
