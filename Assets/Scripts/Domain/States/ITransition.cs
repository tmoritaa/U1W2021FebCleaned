namespace Core.States.Domain {
  public interface ITransition {
    IState NextState { get; }

    bool CanTransition();
  }
}