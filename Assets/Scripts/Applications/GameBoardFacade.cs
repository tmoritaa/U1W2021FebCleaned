using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;

namespace Applications {
  public class GameBoardFacade : IDisposable {
    private const int NUM_PIECES_FOR_SCORE = 3;
    private const int TIME_PENALTY_FOR_FAIL = 10;

    public readonly GameBoard GameBoard; // TODO: for now

    private readonly IBoardPresenter presenter;
    private readonly GameScorer gameScorer;
    private readonly IScorePresenter scorePresenter;
    private readonly GameTimer gameTimer;

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public GameBoardFacade(GameBoard gameBoard, GameTimer gameTimer, IBoardPresenter presenter, GameScorer gameScorer, IScorePresenter scorePresenter) {
      this.GameBoard = gameBoard;
      this.gameTimer = gameTimer;
      this.presenter = presenter;
      this.gameScorer = gameScorer;
      this.scorePresenter = scorePresenter;
    }

    public int NumScoring { get; private set; }

    public void Initialize() {
      this.presenter.Initialize(this.GameBoard);
    }

    public bool HasPiecesAt(Vector2Int coord) {
      return this.GameBoard[coord].Pieces.Count > 0;
    }

    public IReadOnlyList<Piece> GetPiecesAndClearForPickup(Vector2Int coord) {
      var tile = this.GameBoard[coord];

      var retList = new List<Piece>(tile.Pieces);

      this.ClearPiecesForTile(tile);
      this.presenter.UpdateTile(tile);

      return retList;
    }

    public void AddPiecesViaThrow(IEnumerable<TileThrowData> datas) {
      var tilesToScore = new List<Tile>();

      foreach (var data in datas) {
        var tile = this.GameBoard[data.Coord];

        foreach (var piece in data.PiecesToAdd) {
          tile.AddPiece(piece);
        }
        this.presenter.UpdateTile(tile);

        if (tile.Pieces.Count >= NUM_PIECES_FOR_SCORE) {
          tilesToScore.Add(tile);
        }
      }

      if (tilesToScore.Count > 0) {
        foreach (var tile in tilesToScore) {
          tile.Reservations.RequestReservation(ReservationType.Scoring);
        }
        this.StartScoring(tilesToScore, this.tokenSource.Token).Forget();
      } else {
        this.gameScorer.DecrementCombo(); // NOTE: resets combo if move didn't result in scoring
        this.scorePresenter.UpdateCombo(this.gameScorer.ComboCount);
      }
    }

    public bool IsCoordInBounds(in Vector2Int coord) {
      return coord.x >= 0 && coord.x < this.GameBoard.Width && coord.y >= 0 && coord.y < this.GameBoard.Height;
    }

    private void ClearPiecesForTile(Tile tile) {
      tile.ClearPieces();
    }

    private async UniTaskVoid StartScoring(IEnumerable<Tile> _tilesToScore, CancellationToken token) {
      this.NumScoring += 1;
      var tilesToScore = _tilesToScore.ToArray();
      await UniTask.SwitchToMainThread(PlayerLoopTiming.Update, token);

      var scoreTasks = new List<UniTask<int>>();
      foreach (var tile in tilesToScore) {
        var task = this.StartScoring(tile, token);
        scoreTasks.Add(task);
      }

      var results = await UniTask.WhenAll(scoreTasks);

      var failedMatchCount = results.Count(s => s <= 0);
      var isPenalty = failedMatchCount > 0;
      var comboAmount = isPenalty ? 1 : this.gameScorer.ComboCount;

      var totalScore = 0;
      foreach (var score in results) {
        if (score >= 0) {
          totalScore += score * comboAmount;
        } else {
          totalScore += score;
        }
      }
      this.gameScorer.AddScore(totalScore);

      List<UniTask> scoringAnimTasks = new List<UniTask>();

      for (int i = 0; i < tilesToScore.Length; ++i) {
        var tile = tilesToScore[i];
        var score = results[i];

        tile.ClearPieces();
        var task = this.presenter.AnimateForScoring(tile, score, comboAmount, token);
        scoringAnimTasks.Add(task);
      }

      if (isPenalty) {
        // TODO: this is the worst. Basically we know when the animate scoring task gets to showing the BAD stuff
        await UniTask.Delay(750, cancellationToken: token);

        this.gameTimer.SubtractTime(TIME_PENALTY_FOR_FAIL * failedMatchCount);
      }

      await UniTask.WhenAll(scoringAnimTasks);

      if (isPenalty) {
        this.gameScorer.ResetCombo();
      } else {
        this.gameScorer.IncrementCombo();
      }

      this.scorePresenter.UpdateScore(this.gameScorer.Score);
      this.scorePresenter.UpdateCombo(this.gameScorer.ComboCount);

      foreach (var tile in tilesToScore) {
        tile.Reservations.UnReserve(ReservationType.Scoring);
      }
      this.NumScoring -= 1;
    }

    private async UniTask<int> StartScoring(Tile tile, CancellationToken token) {
      await UniTask.WaitUntil(() => tile.Reservations.CanReserve(ReservationType.Scoring), cancellationToken: token);

      tile.Reservations.Reserve(ReservationType.Scoring);

      return this.gameScorer.CalculateBaseScore(tile);
    }

    public void Dispose() {
      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();
      this.tokenSource = null;
    }
  }
}