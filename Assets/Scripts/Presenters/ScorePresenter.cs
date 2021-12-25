using Applications;
using Views;

namespace Presenters {
  public class ScorePresenter : IScorePresenter {
    private readonly ScoreView scoreView;

    public ScorePresenter(ScoreView scoreView) {
      this.scoreView = scoreView;
    }

    public void UpdateScore(int score) {
      this.scoreView.UpdateScore(score);
    }

    public void UpdateCombo(int combo) {
      this.scoreView.UpdateCombo(combo);
    }
  }
}