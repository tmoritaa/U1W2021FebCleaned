using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters {
  public class IntroOutroPresenter : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image overlay;

    public void PlayIntro(string text, Action onComplete) {
      this.text.text = text;
      var origOverlayColor = this.overlay.color;
      this.overlay.gameObject.SetActive(true);

      var sequence = DOTween.Sequence();
      sequence
        .Append(this.text.DOFade(1f, 0.75f))
        .Append(this.text.DOFade(0f, 0.75f))
        .Insert(1, this.overlay.DOFade(0f, 0.75f))
        .OnComplete(() => {
          this.overlay.color = origOverlayColor;
          this.overlay.gameObject.SetActive(false);
          onComplete();
        });
    }

    public void PlayOutro(string text, Action onComplete) {
      this.text.text = text;
      var origOverlayColor = this.overlay.color;
      this.overlay.gameObject.SetActive(true);

      this.overlay.color = new Color(origOverlayColor.r, origOverlayColor.g, origOverlayColor.b, 0);

      var sequence = DOTween.Sequence();
      sequence
        .Append(this.text.DOFade(1f, 0.75f))
        .Insert(0, this.overlay.DOFade(origOverlayColor.a, 0.75f))
        .AppendInterval(1f)
        .Append(this.text.DOFade(0f, 0.75f))
        .OnComplete(() => {
          onComplete();
        });
    }
  }
}