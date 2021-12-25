using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using TMPro;
using UnityEngine;
using VContainer;

namespace Views {
  public class TileView : MonoBehaviour {
    [SerializeField] private List<PieceView> stackedPieceViews;
    [SerializeField] private SpriteRenderer boxSprite;
    [SerializeField] private float yOffsetOnBoxSpriteAnim = 0.3f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private Color badTextColor;

    private SEPlayer sePlayer;

    private Tile tile;

    // TODO: For now. Generally speaking I don't like that this has direct reference to Tile
    public void Initialize(Tile tile, SEPlayer sePlayer) {
      this.tile = tile;
      this.sePlayer = sePlayer;

      this.UpdateView();
    }

    public void UpdateView() {
      var piecesOnTile = this.tile.Pieces;
      for (int i = 0; i < this.stackedPieceViews.Count; ++i) {
        var stackedPieceView = this.stackedPieceViews[i];
        if (i < piecesOnTile.Count) {
          stackedPieceView.UpdateAndShow(piecesOnTile[i]);
        } else {
          stackedPieceView.Hide();
        }
      }
    }

    public async UniTask AnimateForScoring(int score, int comboCount, CancellationToken token) {
      var boxTransform = this.boxSprite.transform;

      var curBoxPos = boxTransform.localPosition;
      var origScale = boxTransform.localScale;
      boxTransform.localPosition = curBoxPos + new Vector3(0, this.yOffsetOnBoxSpriteAnim, 0);

      this.boxSprite.DOFade(1f, 0.2f);
      this.sePlayer.RequestPlayAudio("box_drop");
      await boxTransform.DOLocalMoveY(curBoxPos.y, 0.3f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);

      this.UpdateView();

      this.sePlayer.RequestPlayAudio("box_shake");
      await boxTransform.DOShakePosition(0.3f, new Vector3(0.1f, 0.1f, 0), 50).ToUniTask(cancellationToken: token);

      await UniTask.Delay(150, cancellationToken: token);

      var scoreTextOrigPos = this.scoreText.transform.localPosition;
      var comboTextOrigPos = this.comboText.transform.localPosition;
      this.scoreText.gameObject.SetActive(true);
      if (score <= 0) {
        this.scoreText.text = "BAD";
        this.scoreText.color = this.badTextColor;

        this.sePlayer.RequestPlayAudio("score_negative");
      } else {
        this.scoreText.text = $"+{score}";

        if (comboCount > 1) {
          this.comboText.gameObject.SetActive(true);
          this.comboText.text = $"x{comboCount}";
        }

        this.scoreText.color = Color.white;

        this.sePlayer.RequestPlayAudio("score_positive");
      }

      this.comboText.transform.DOLocalMoveY(comboTextOrigPos.y + 0.1f, 0.4f);
      this.scoreText.transform.DOLocalMoveY(scoreTextOrigPos.y + 0.1f, 0.4f);

      this.boxSprite.DOFade(0f, 0.3f);
      boxTransform.DOScaleX(0f, 0.2f).SetEase(Ease.OutExpo);
      await boxTransform.DOLocalMoveY(curBoxPos.y + this.yOffsetOnBoxSpriteAnim, 0.3f).ToUniTask(cancellationToken: token);

      await UniTask.Delay(350, cancellationToken: token);

      boxTransform.localScale = origScale;
      boxTransform.localPosition = curBoxPos;

      this.scoreText.gameObject.SetActive(false);
      this.comboText.gameObject.SetActive(false);
      this.scoreText.transform.localPosition = scoreTextOrigPos;
      this.comboText.transform.localPosition = comboTextOrigPos;
    }
  }
}