using System;
using System.Threading;
using Applications;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;
using Views;

namespace Presenters {
  public class BoardPresenter : IBoardPresenter {
    private readonly BoardView boardView;
    private readonly SEPlayer sePlayer;

    public IObservable<InputData> OnInput => this.boardView.OnInput;
    public IObservable<Unit> OnRestartInput => this.boardView.OnRestartInput;

    public BoardPresenter(BoardView boardView, SEPlayer sePlayer) {
      this.boardView = boardView;
      this.sePlayer = sePlayer;
    }

    public void Initialize(GameBoard gameBoard) {
      this.boardView.Initialize(gameBoard);
    }

    public void UpdateTile(Tile tile) {
      this.boardView.UpdateTile(tile);

      if (tile.Pieces.Count > 0) {
        this.sePlayer.RequestPlayAudio("book_drop");
      }
    }

    public void ToggleInputPointDueToRun(bool b, Vector2Int coord) {
      this.boardView.ToggleInputPointDueToRun(b, coord);
    }

    public void ToggleAllInputPoints(bool b) {
      this.boardView.ToggleAllInputPoints(b);
    }

    public async UniTask AnimateForScoring(Tile tile, int score, int comboCount, CancellationToken token) {
      await this.boardView.AnimateTileForScoring(tile, score, comboCount, token);
    }
  }
}