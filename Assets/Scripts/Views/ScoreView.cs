using TMPro;
using UnityEngine;

namespace Views {
  public class ScoreView : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;

    public void UpdateScore(int score) {
      this.scoreText.text = $"{score}";
    }

    public void UpdateCombo(int combo) {
      this.comboText.text = $"x{combo}";
    }
  }
}