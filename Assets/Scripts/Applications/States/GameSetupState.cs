using Core.States.Domain;
using Domain;
using UniRx;

namespace Applications.States {
  public class GameSetupState : BaseTransitionTriggerState {
    private readonly GameBoardFacade gameBoard;
    private readonly GameContext gameContext;
    private readonly GameScorer gameScorer;
    private readonly IScorePresenter scorePresenter;
    private readonly GameTimer gameTimer;
    private readonly ITimerPresenter timerPresenter;

    public GameSetupState(GameContext gameContext, GameTimer gameTimer, GameBoardFacade gameBoard, GameScorer gameScorer, IScorePresenter scorePresenter, ITimerPresenter timerPresenter) {
      this.gameContext = gameContext;
      this.gameTimer = gameTimer;
      this.gameBoard = gameBoard;
      this.gameScorer = gameScorer;
      this.scorePresenter = scorePresenter;
      this.timerPresenter = timerPresenter;
    }

    protected override void PerformOnEnter() {
      this.gameBoard.Initialize();

      this.scorePresenter.UpdateScore(this.gameScorer.Score);
      this.scorePresenter.UpdateCombo(this.gameScorer.ComboCount);

      this.timerPresenter.UpdateTime(this.gameTimer.CurTime, 0);

      for (int i = 0; i < 3; ++i) {
        this.gameContext.AddNewCollectorToQueue();
      }

      this.completeSubject.OnNext(Unit.Default);
    }

    protected override void PerformOnExit() {
    }
  }
}