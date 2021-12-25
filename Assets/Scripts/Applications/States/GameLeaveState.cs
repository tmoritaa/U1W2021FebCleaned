using Core.States.Domain;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Applications.States {
  public class GameLeaveState : BaseState {
    private readonly GameContext gameContext;
    private readonly Image image;

    public GameLeaveState(GameContext context, Image image) {
      this.gameContext = context;
      this.image = image;
    }

    protected override void PerformOnEnter() {
      this.image.DOFade(1f, 0.75f)
        .OnComplete(() => SceneManager.LoadScene(this.gameContext.NextScene));
    }

    protected override void PerformOnExit() {
    }
  }
}