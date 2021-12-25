using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using UnityEngine;
using UniRx;

namespace Views {
  public class CollectorView : MonoBehaviour {
    [SerializeField] private List<PieceView> pieceViews;

    [SerializeField] private CollectorAnimator collectorAnimator;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public void StartRun(Vector3 startPos, bool facingLeft) {
      this.UpdatePos(startPos);
      this.spriteRenderer.flipX = !facingLeft;
      this.TriggerStartRunAnim();
    }

    public void TriggerStartRunAnim() {
      this.collectorAnimator.TriggerStartRun();
    }

    public void TriggerRunAnim() {
      this.collectorAnimator.TriggerRun();
    }

    public async UniTask StartTripping(Vector2 moveDir, CancellationToken token) {
      await this.collectorAnimator.DoTripping(moveDir, token);
    }

    public void ReparentTo(Transform transform) {
      this.transform.SetParent(transform, false);
    }

    public void UpdatePos(Vector3 pos) {
      this.gameObject.transform.localPosition = pos;
    }

    public void UpdatePieces(IReadOnlyList<Piece> newHeldPieces) {
      for (int i = 0; i < this.pieceViews.Count; ++i) {
        if (i < newHeldPieces.Count) {
          var piece = newHeldPieces[i];

          this.pieceViews[i].gameObject.SetActive(true);
          this.pieceViews[i].UpdateAndShow(piece);
        } else {
          this.pieceViews[i].gameObject.SetActive(false);
        }
      }
    }
  }
}