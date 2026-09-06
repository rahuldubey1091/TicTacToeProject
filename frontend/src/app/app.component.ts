import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GameMode, GameService, GameState, Player } from './game.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly gameService = inject(GameService);

  game: GameState | null = null;
  selectedMode: GameMode = 'TwoPlayer';
  error = '';
  busy = false;

  ngOnInit(): void {
    this.startNewGame();
  }

  startNewGame(): void {
    this.error = '';
    this.busy = true;
    this.gameService.createGame(this.selectedMode).subscribe({
      next: state => {
        this.game = state;
        this.busy = false;
      },
      error: err => this.handleError(err)
    });
  }

  onModeChange(): void {
    this.startNewGame();
  }

  play(row: number, column: number): void {
    if (!this.game || this.busy) return;
    if (this.game.gameStatus !== 'InProgress') return;
    if (this.game.board[row][column]) return;

    this.error = '';
    this.busy = true;
    this.gameService.move(this.game.gameId, this.game.currentPlayer, row, column).subscribe({
      next: state => {
        this.game = state;
        this.busy = false;
      },
      error: err => this.handleError(err)
    });
  }

  undo(): void {
    if (!this.game || this.busy || this.game.moveHistory.length === 0) return;

    this.error = '';
    this.busy = true;
    this.gameService.undo(this.game.gameId).subscribe({
      next: state => {
        this.game = state;
        this.busy = false;
      },
      error: err => this.handleError(err)
    });
  }

  resetGame(): void {
    if (!this.game || this.busy) return;

    this.error = '';
    this.busy = true;
    this.gameService.resetGame(this.game.gameId).subscribe({
      next: state => {
        this.game = state;
        this.busy = false;
      },
      error: err => this.handleError(err)
    });
  }

  resetScoreboard(): void {
    if (!this.game || this.busy) return;

    this.error = '';
    this.busy = true;
    this.gameService.resetScoreboard().subscribe({
      next: scoreboard => {
        this.game = { ...this.game!, scoreboard };
        this.busy = false;
      },
      error: err => this.handleError(err)
    });
  }

  isWinningCell(row: number, column: number): boolean {
    return this.game?.winningCells.some(cell => cell[0] === row && cell[1] === column) ?? false;
  }

  cellLabel(row: number, column: number): string {
    return `Row ${row + 1}, Column ${column + 1}`;
  }

  trackByIndex(index: number): number {
    return index;
  }

  private handleError(err: any): void {
    this.busy = false;
    this.error = err?.error?.error ?? 'Something went wrong. Please try again.';
  }
}
