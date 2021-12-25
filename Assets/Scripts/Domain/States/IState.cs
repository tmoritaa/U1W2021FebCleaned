using System;

namespace Core.States.Domain {
  public interface IState : IDisposable {
    void OnEnter();
    void OnExit();

    bool CanTransition();

    IState GetStateToTransition();
  }
}
