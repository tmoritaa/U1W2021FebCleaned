using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domain {
  public interface ICollectorPresenter {
    void Initialize(Collector collector);

    UniTask StartRunning(Collector collector, CancellationToken cancellationToken);

    UniTask StartTripping(Collector collector, CancellationToken token);

    void TriggerStartRunAnim();

    void TriggerRunAnim();

    void UpdateForQueueDisplay(Vector2 pos, Transform newParent);

    void Hide();

    void UpdatePosForMove(Collector collector);

    void UpdatePieces(Collector collector);

    UniTask Completed(CancellationToken token);
  }
}