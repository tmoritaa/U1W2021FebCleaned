using Core.States.Domain;
using UniRx;
using VContainer.Unity;

namespace Applications.States {
  public class GameStateMachine : AsyncStateMachine, IStartable {
    public GameStateMachine(
      GameSetupState setupState,
      GameIntroState introState,
      GameGameplayState gameplayState,
      GameResultState resultState,
      GameLeaveState leaveState) : base(setupState, new IState[] { setupState, gameplayState, resultState }) {
      setupState.AddTransition(new Transition(introState, () => true));
      introState.AddTransition(new Transition(gameplayState, () => true));
      gameplayState.AddTransition(new Transition(resultState, () => true));
      resultState.AddTransition(new Transition(leaveState, () => true));

      setupState.OnComplete.Subscribe(_ => this.SetupTransitionAtNextLateUpdate());
      introState.OnComplete.Subscribe(_ => this.SetupTransitionAtNextLateUpdate());
      gameplayState.OnComplete.Subscribe(_ => this.SetupTransitionAtNextLateUpdate());
      resultState.OnComplete.Subscribe(_ => this.SetupTransitionAtNextLateUpdate());
    }

    public void Start() {
      this.StartRunning();
    }
  }
}