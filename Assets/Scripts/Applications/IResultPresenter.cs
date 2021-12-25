using System;
using UniRx;

namespace Applications {
  public interface IResultPresenter {
    IObservable<Unit> OnHome { get; }
    IObservable<Unit> OnReplay { get; }

    void Show();
  }
}