using System;
using UniRx;

namespace Core.States.Domain {
  public class StateMachine : IDisposable {
    private readonly IState startState = null;

    protected IState curState;

    protected IState[] states;

    public bool Running { get; private set; }

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public StateMachine(IState startState, IState[] states) {
      this.startState = startState;

      foreach (var state in states) {
        this.disposables.Add(state);
      }
    }

    public virtual void StartRunning() {
      this.curState = this.startState;

      this.Running = true;

      this.curState.OnEnter();
    }

    public virtual void StopRunning() {
      this.Running = false;

      this.curState = null;
    }

    public virtual void Dispose() {
      this.disposables.Clear();
    }

    protected bool TransitionIfPossible() {
      if (!this.Running) {
        throw new InvalidOperationException("GameStateTracker instructed to try transition before starting");
      }

      bool transitioned = false;
      if (this.curState.CanTransition()) {
        var nextState = this.curState.GetStateToTransition();
        this.curState.OnExit();

        this.curState = nextState;
        this.curState.OnEnter();

        transitioned = true;
      }

      return transitioned;
    }
  }
}
