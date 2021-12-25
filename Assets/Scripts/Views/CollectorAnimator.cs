using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extensions;
using UniRx;
using UnityEngine;

namespace Views {
  public class CollectorAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;

    private readonly Subject<Unit> animCompleteSubject = new Subject<Unit>();

    public IObservable<Unit> OnAnimComplete => this.animCompleteSubject;

    public void TriggerStartRun() {
      this.animator.SetTrigger("start_run");
    }

    public void TriggerRun() {
      this.animator.SetTrigger("run");
    }

    public async UniTask DoTripping(Vector3 moveDir, CancellationToken token) {
      this.animator.SetTrigger("tripping");

      Vector3 pos = this.transform.localPosition;
      await this.transform.DOLocalMove(pos + moveDir * 0.2f, 0.5f).ToUniTask(cancellationToken: token);
    }

    public void AnimationComplete() {
      this.animCompleteSubject.OnNext(Unit.Default);
    }
  }
}