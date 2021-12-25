using Core.States.Domain;
using DG.Tweening;
using Presenters;
using UniRx;
using UnityEngine.UI;

namespace Applications.States {
  public class GameIntroState : BaseTransitionTriggerState {
    private readonly Image image;
    private readonly IntroOutroPresenter introOutroPresenter;

    public GameIntroState(Image image, IntroOutroPresenter outroPresenter) {
      this.image = image;
      this.introOutroPresenter = outroPresenter;
    }

    protected override void PerformOnEnter() {
      this.image.DOFade(0f, 0.5f)
        .OnComplete(
          () => {
            this.introOutroPresenter.PlayIntro("Start", () => this.completeSubject.OnNext(Unit.Default));
          });
    }

    protected override void PerformOnExit() {
    }
  }
}