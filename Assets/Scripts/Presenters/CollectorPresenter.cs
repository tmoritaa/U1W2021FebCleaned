using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Applications;
using Cysharp.Threading.Tasks;
using Domain;
using Extensions;
using UniRx;
using UnityEngine;
using Views;

namespace Presenters {
  public class CollectorPresenter : ICollectorPresenter {
    private readonly CollectorView view;
    private readonly PosTranslator posTranslator;
    private readonly Transform collectorRoot;
    private readonly SEPlayer sePlayer;

    public CollectorPresenter(CollectorView view, PosTranslator posTranslator, Transform collectorRoot, SEPlayer sePlayer) {
      this.view = view;
      this.posTranslator = posTranslator;
      this.collectorRoot = collectorRoot;
      this.sePlayer = sePlayer;
    }

    public void Initialize(Collector collector) {
      this.view.gameObject.SetActive(false);

      this.view.UpdatePieces(collector.HeldPieces);
    }

    public async UniTask StartRunning(Collector collector, CancellationToken cancellationToken) {
      Vector3 pos = this.posTranslator.CalculatePosFromCoord(collector.CurCoord);

      this.view.ReparentTo(this.collectorRoot);

      bool facingLeft = collector.MoveDir.x < 0 || collector.MoveDir.y < 0;
      this.view.gameObject.SetActive(true);

      this.sePlayer.RequestPlayAudio("collector_enter");
      this.view.StartRun(pos, facingLeft);
      await UniTask.Delay(500, cancellationToken: cancellationToken);
    }

    public async UniTask StartTripping(Collector collector, CancellationToken token) {
      var dir = collector.MoveDir;
      dir.y *= -1;
      await this.view.StartTripping(((Vector2)dir).Rotate(45), token);
    }

    public void TriggerStartRunAnim() {
      this.view.TriggerStartRunAnim();
    }

    public void TriggerRunAnim() {
      this.view.TriggerRunAnim();
    }

    public void UpdateForQueueDisplay(Vector2 pos, Transform newParent) {
      this.view.ReparentTo(newParent);
      this.view.gameObject.SetActive(true);
      this.view.UpdatePos(pos);
    }

    public void Hide() {
      this.view.gameObject.SetActive(false);
    }

    public void UpdatePosForMove(Collector collector) {
      var srcPos = this.posTranslator.CalculatePosFromCoord(collector.CurCoord);
      var tgtPos = this.posTranslator.CalculatePosFromCoord(collector.NextCoord);

      var pos = Vector3.Lerp(srcPos, tgtPos, Mathf.Clamp01(collector.TravelPercent));

      this.view.UpdatePos(pos);
    }

    public void UpdatePieces(Collector collector) {
      this.view.UpdatePieces(collector.HeldPieces);
    }

    public async UniTask Completed(CancellationToken token) {
      await UniTask.DelayFrame(1, cancellationToken: token);
      GameObject.Destroy(this.view.gameObject);
    }
  }
}