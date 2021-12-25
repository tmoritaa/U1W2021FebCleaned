using System;
using Applications;
using Cysharp.Threading.Tasks;
using UniRx;
using Views;

namespace Presenters {
  public class ResultPresenter : IResultPresenter {
    private readonly ResultModal resultModal;

    public ResultPresenter(ResultModal resultModal) {
      this.resultModal = resultModal;
    }

    public IObservable<Unit> OnHome => this.resultModal.OnHome;
    public IObservable<Unit> OnReplay => resultModal.OnReplay;

    public void Show() {
      this.resultModal.Show();
    }
  }
}