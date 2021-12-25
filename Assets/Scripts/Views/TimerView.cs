using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views {
  public class TimerView : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text penaltyText;

    public void UpdateTime(int seconds, int changedVal) {
      this.text.text = seconds.ToString();

      if (changedVal > 1) {
        DOTween.Kill("timer_penalty");

        this.penaltyText.gameObject.SetActive(true);
        this.penaltyText.alpha = 0f;

        this.penaltyText.text = $"-{changedVal}";

        var origPos = this.penaltyText.transform.localPosition;
        this.penaltyText.DOFade(1f, 0.5f)
          .SetId("timer_penalty");
        this.penaltyText.transform.DOLocalMoveY(origPos.y + 5f, 0.75f)
          .OnComplete(() => {
            this.penaltyText.transform.localPosition = origPos;
            this.penaltyText.gameObject.SetActive(false);
          }).SetId("timer_penalty");
      }
    }
  }
}