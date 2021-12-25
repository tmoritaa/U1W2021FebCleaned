using System.Collections.Generic;
using System.Threading;
using Core.States.Domain;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Views;

namespace Applications.States {
  public class GameGameplayState : BaseTransitionTriggerState {
    private readonly IBoardPresenter boardPresenter;
    private readonly ITimerPresenter timerPresenter;
    private readonly ICollectorQueuePresenter collectorQueuePresenter;
    private readonly GameBoardFacade gameBoardFacade;

    private readonly List<CollectorFacade> runningCollectors = new List<CollectorFacade>();

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly GameContext gameContext;
    private readonly GameTimer gameTimer;

    private CancellationTokenSource tokenSource;

    public GameGameplayState(GameContext gameContext, GameTimer gameTimer, IBoardPresenter boardPresenter, ITimerPresenter timerPresenter, ICollectorQueuePresenter collectorQueuePresenter, GameBoardFacade gameBoardFacade) {
      this.gameContext = gameContext;
      this.gameTimer = gameTimer;
      this.boardPresenter = boardPresenter;
      this.timerPresenter = timerPresenter;
      this.collectorQueuePresenter = collectorQueuePresenter;
      this.gameBoardFacade = gameBoardFacade;
    }

    private bool TimerEnded => this.gameTimer.CurTime <= 0;

    public override void Dispose() {
      this.disposables.Clear();
      foreach (var collector in this.runningCollectors) {
        collector.Dispose();
      }

      this.runningCollectors.Clear();

      foreach (var collector in this.gameContext.CollectorBag) {
        collector.Dispose();
      }
      this.gameContext.CollectorBag.Clear();

      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();
      this.tokenSource = null;
    }

    protected override void PerformOnEnter() {
      this.boardPresenter.OnInput
        .Where(_ => this.gameContext.CollectorBag.Count > 0 && !this.TimerEnded)
        .Subscribe(this.StartNewCollector)
        .AddTo(this.disposables);

      this.boardPresenter.OnRestartInput
        .Subscribe(_ => this.RestartScene())
        .AddTo(this.disposables);

      this.gameTimer.OnTimeUpdate
        .Subscribe(timeChange => this.timerPresenter.UpdateTime(timeChange.Item1, timeChange.Item2))
        .AddTo(this.disposables);

      this.tokenSource = new CancellationTokenSource();
      this.PollGameCompletion(this.tokenSource.Token).Forget();
    }

    protected override void PerformOnExit() {
      this.Dispose();
    }

    private void StartNewCollector(InputData inputData) {
      var collector = this.gameContext.CollectorBag.Dequeue();

      var startingCoord = inputData.Coord;
      collector.OnLoopComplete.Subscribe(
        _ => { },
        () => {
          this.runningCollectors.Remove(collector);

          if (!this.TimerEnded) {
            this.gameContext.AddNewCollectorToQueue();
            this.ToggleOpposingInputPoints(true, startingCoord);
            this.boardPresenter.ToggleAllInputPoints(true);
          }
        });

      this.collectorQueuePresenter.DequeueCollectorDisplay();

      this.ToggleOpposingInputPoints(false, startingCoord);

      if (this.gameContext.CollectorBag.Count == 0) {
        this.boardPresenter.ToggleAllInputPoints(false);
      }

      collector.Start(inputData.Coord, inputData.FacingDir);

      this.runningCollectors.Add(collector);
    }

    private async UniTaskVoid PollGameCompletion(CancellationToken token) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.Update, token);

      while (this.gameTimer.CurTime > 0) {
        await UniTask.Delay(1000, cancellationToken: token);
        this.gameTimer.SubtractTime(1);
      }

      this.boardPresenter.ToggleAllInputPoints(false);

      await UniTask.WaitUntil(() => this.runningCollectors.Count == 0 && this.gameBoardFacade.NumScoring == 0, cancellationToken: token);

      this.completeSubject.OnNext(Unit.Default);
    }

    private void ToggleOpposingInputPoints(bool b, Vector2Int coord) {
      this.boardPresenter.ToggleInputPointDueToRun(b, coord);
      this.boardPresenter.ToggleInputPointDueToRun(b, this.CalcOpposingInputPoint(coord));
    }

    private Vector2Int CalcOpposingInputPoint(Vector2Int coord) {
      bool xInBounds = coord.x >= 0 && coord.x < this.gameBoardFacade.GameBoard.Width;
      bool yInBounds = coord.y >= 0 && coord.y < this.gameBoardFacade.GameBoard.Height;

      var newCoord = coord;
      if (xInBounds) {
        newCoord.y = newCoord.y >= 0 ? -1 : this.gameBoardFacade.GameBoard.Height;
      } else if (yInBounds) {
        newCoord.x = newCoord.x >= 0 ? -1 : this.gameBoardFacade.GameBoard.Width;
      }

      return newCoord;
    }

    private void RestartScene() {
      SceneManager.LoadScene("Game");
    }
  }
}