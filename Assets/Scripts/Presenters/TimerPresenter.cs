using Applications;
using Views;

namespace Presenters {
  public class TimerPresenter : ITimerPresenter {
    private readonly TimerView timerView;

    public TimerPresenter(TimerView timerView) {
      this.timerView = timerView;
    }

    public void UpdateTime(int seconds, int changedVal) {
      this.timerView.UpdateTime(seconds, changedVal);
    }
  }
}