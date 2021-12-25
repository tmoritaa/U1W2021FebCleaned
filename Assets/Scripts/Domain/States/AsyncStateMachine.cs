using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.States.Domain {
  public abstract class AsyncStateMachine : StateMachine {
    private CancellationTokenSource tokenSource;

    public AsyncStateMachine(IState startState, IState[] states) : base(startState, states) {
    }

    protected void SetupTransitionAtNextLateUpdate() {
      this.tokenSource = new CancellationTokenSource();

      this.TransitionAtNextLateUpdate(this.tokenSource.Token).Forget();
    }

    private async UniTaskVoid TransitionAtNextLateUpdate(CancellationToken token) {
      await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);

      this.TransitionIfPossible();
    }

    public override void Dispose() {
      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();

      base.Dispose();
    }
  }
}