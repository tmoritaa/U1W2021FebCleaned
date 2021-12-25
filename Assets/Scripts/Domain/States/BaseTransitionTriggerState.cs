using System;
using UniRx;

namespace Core.States.Domain {
  public abstract class BaseTransitionTriggerState : BaseState {
    protected readonly Subject<Unit> completeSubject = new Subject<Unit>();

    public IObservable<Unit> OnComplete => this.completeSubject;

    public override void Dispose() {
      this.completeSubject.Dispose();
    }
  }
}