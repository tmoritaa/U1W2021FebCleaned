using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.States.Domain {
  public abstract class BaseState : IState, IDisposable {
    private List<ITransition> transitions = new List<ITransition>();

    protected bool IsRunning { get; private set; } = false;

    public void OnEnter() {
      this.IsRunning = true;
      this.PerformOnEnter();
    }

    public void OnExit() {
      this.IsRunning = false;
      this.PerformOnExit();
    }

    public bool CanTransition() => this.transitions.Any(t => t.CanTransition());

    public IState GetStateToTransition() {
      var validTransitions = this.transitions.FindAll(t => t.CanTransition());

      if (validTransitions.Count != 1) {
        throw validTransitions.Count > 1 ?
            new InvalidOperationException("Multiple valid transitions exist") : new InvalidOperationException("No valid transitions exist");
      }

      return validTransitions.First().NextState;
    }

    public void AddTransition(ITransition transition) => this.transitions.Add(transition);

    public virtual void Dispose() {}

    protected abstract void PerformOnEnter();

    protected abstract void PerformOnExit();
  }

  public class Transition : ITransition {
    private readonly IState state;
    private readonly Func<bool> condition;

    public Transition(IState state, Func<bool> condition) => (this.state, this.condition) = (state, condition);

    public IState NextState => this.state;

    public bool CanTransition() {
      return this.condition();
    }
  }
}
