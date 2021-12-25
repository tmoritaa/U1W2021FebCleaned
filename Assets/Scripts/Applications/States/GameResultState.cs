using Core.States.Domain;
using Domain;
using naichilab;
using Presenters;
using UniRx;

namespace Applications.States {
  public class GameResultState : BaseTransitionTriggerState {
    private readonly GameContext gameContext;
    private readonly IResultPresenter presenter;
    private readonly GameScorer gameScorer;
    private readonly IntroOutroPresenter introOutroPresenter;

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameResultState(GameContext gameContext, IntroOutroPresenter introOutroPresenter, IResultPresenter presenter, GameScorer gameScorer) {
      this.gameContext = gameContext;
      this.presenter = presenter;
      this.introOutroPresenter = introOutroPresenter;
      this.gameScorer = gameScorer;
    }

    public override void Dispose() {
      this.disposables.Clear();
    }

    protected override void PerformOnEnter() {
      this.introOutroPresenter.PlayOutro("Time Over",
        () => {
          this.presenter.OnHome
            .Subscribe(__ => {
              this.gameContext.SetNextScene("Start");
              this.completeSubject.OnNext(__);
            })
            .AddTo(this.disposables);

          this.presenter.OnReplay
            .Subscribe(__ => {
              this.gameContext.SetNextScene("Game");
              this.completeSubject.OnNext(__);
            })
            .AddTo(this.disposables);

          RankingLoader.Instance.SendScoreAndShowRanking(this.gameScorer.Score);

          this.presenter.Show();
        });
    }

    protected override void PerformOnExit() {
      this.disposables.Clear();
    }
  }
}