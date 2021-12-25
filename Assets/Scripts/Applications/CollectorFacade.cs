using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;

namespace Applications {
  public class CollectorFacade : IDisposable {
    public readonly ICollectorPresenter presenter;
    private readonly Collector collector;
    private readonly GameBoardFacade gameBoardFacade;

    private Subject<Unit> loopCompleteSubject = new Subject<Unit>();

    private CancellationTokenSource tokenSource;

    public CollectorFacade(Collector collector, ICollectorPresenter presenter, GameBoardFacade gameBoardFacade) {
      this.collector = collector;
      this.presenter = presenter;
      this.gameBoardFacade = gameBoardFacade;
    }

    public IObservable<Unit> OnLoopComplete => this.loopCompleteSubject;

    public void Dispose() {
      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();
      this.tokenSource = null;

      this.loopCompleteSubject.Dispose();
    }

    public void Initialize() {
      this.presenter.Initialize(this.collector);
    }

    public void Start(Vector2Int startingCoord, Vector2Int facingDir) {
      this.collector.PrepareForStart(startingCoord, facingDir);

      this.tokenSource = new CancellationTokenSource();

      this.PerformLoopLogic(this.tokenSource.Token).Forget();
    }

    private async UniTaskVoid PerformLoopLogic(CancellationToken token) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.Update, token);

      await this.StartOperation(token);

      for (int i = 0; i < 2; ++i) {
        await this.MoveToNextCoord(token);
        await this.PickupAtCoord(token);
      }

      await this.FallDownAndDropPieces(token);

      await this.presenter.Completed(token);
      this.gameBoardFacade.GameBoard[this.collector.CurCoord].Reservations.UnReserve(ReservationType.Collector);

      this.loopCompleteSubject.OnCompleted();
    }

    private async UniTask StartOperation(CancellationToken token) {
      await this.presenter.StartRunning(this.collector, token);
    }

    private async UniTask MoveToNextCoord(CancellationToken token) {
      this.collector.SetupForNextMove();

      var nextTile = this.gameBoardFacade.GameBoard[this.collector.NextCoord];

      nextTile.Reservations.RequestReservation(ReservationType.Collector);

      while (!nextTile.Reservations.CanReserve(ReservationType.Collector)) {
        this.presenter.TriggerStartRunAnim();
        await UniTask.NextFrame(cancellationToken: token);
      }

      nextTile.Reservations.Reserve(ReservationType.Collector);

      this.presenter.TriggerRunAnim();

      while (!this.collector.HasReachedNextCoord) {
        await UniTask.NextFrame(cancellationToken: token);

        var prevPercent = this.collector.TravelPercent;
        var curCoord = this.collector.CurCoord;
        this.collector.MoveTowardsNextCoord(Time.deltaTime);

        if (this.collector.TravelPercent >= 0.5f && prevPercent < 0.5f && this.gameBoardFacade.IsCoordInBounds(curCoord)) {
          this.gameBoardFacade.GameBoard[curCoord].Reservations.UnReserve(ReservationType.Collector);
        }

        this.presenter.UpdatePosForMove(this.collector);
      }
    }

    private async UniTask PickupAtCoord(CancellationToken token) {
      if (this.gameBoardFacade.HasPiecesAt(this.collector.CurCoord)) {
        var pieces = this.gameBoardFacade.GetPiecesAndClearForPickup(this.collector.CurCoord);
        this.collector.AddPieces(pieces);
        this.presenter.UpdatePieces(this.collector);
        await UniTask.Delay(150, cancellationToken: token);
      }
    }

    private async UniTask FallDownAndDropPieces(CancellationToken token) {
      var pieceDropCoords = new[] {
        this.collector.CurCoord + this.collector.MoveDir * 3,
        this.collector.CurCoord + this.collector.MoveDir * 2,
        this.collector.CurCoord + this.collector.MoveDir,
      };

      foreach (var coord in pieceDropCoords) {
        this.gameBoardFacade.GameBoard[coord].Reservations.RequestReservation(ReservationType.Collector);
      }

      while (pieceDropCoords.Any(p => !this.gameBoardFacade.GameBoard[p].Reservations.CanReserve(ReservationType.Collector))) {
        this.presenter.TriggerStartRunAnim();
        await UniTask.DelayFrame(1, cancellationToken: token);
      }

      foreach (var coord in pieceDropCoords) {
        this.gameBoardFacade.GameBoard[coord].Reservations.Reserve(ReservationType.Collector);
      }

      var numPiecesPerDist = new List<int>();
      if (this.collector.HeldPieces.Count % 3 == 0) {
        var num = this.collector.HeldPieces.Count / 3;
        numPiecesPerDist.Add(num);
        numPiecesPerDist.Add(num);
        numPiecesPerDist.Add(num);
      } else {
        numPiecesPerDist.Add(Mathf.FloorToInt(this.collector.HeldPieces.Count / 3.0f));
        numPiecesPerDist.Add(Mathf.RoundToInt(this.collector.HeldPieces.Count / 3.0f));
        numPiecesPerDist.Add(Mathf.CeilToInt(this.collector.HeldPieces.Count / 3.0f));
      }

      int count = 0;
      var tileThrowDatas = new List<TileThrowData>();
      for (int i = 0; i < numPiecesPerDist.Count; ++i) {
        var coord = this.collector.CurCoord + this.collector.MoveDir * (i + 1);

        var piecesToAdd = new List<Piece>();
        for (int j = 0; j < numPiecesPerDist[i]; ++j) {
          piecesToAdd.Add(this.collector.HeldPieces[count]);
          count += 1;
        }

        tileThrowDatas.Add(new TileThrowData(piecesToAdd, coord));
      }

      this.gameBoardFacade.AddPiecesViaThrow(tileThrowDatas);

      this.collector.ClearPieces();

      this.presenter.UpdatePieces(this.collector);
      await this.presenter.StartTripping(this.collector, token);

      foreach (var coord in pieceDropCoords) {
        this.gameBoardFacade.GameBoard[coord].Reservations.UnReserve(ReservationType.Collector);
      }
    }
  }
}