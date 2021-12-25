using System;
using Applications;
using UniRx;
using UnityEngine;

namespace Views {
  public class ResultModal : MonoBehaviour {
    [SerializeField] private GameObject overlayObject;

    private readonly Subject<Unit> homeSubject = new Subject<Unit>();
    private readonly Subject<Unit> replaySubject = new Subject<Unit>();

    public IObservable<Unit> OnHome => this.homeSubject;

    public IObservable<Unit> OnReplay => this.replaySubject;

    public void Show() {
      this.overlayObject.SetActive(true);
      this.gameObject.SetActive(true);
    }

    public void GoHome() {
      this.homeSubject.OnNext(Unit.Default);
    }

    public void GoReplay() {
      this.replaySubject.OnNext(Unit.Default);
    }


    private void OnDestroy() {
      this.homeSubject.Dispose();
    }
  }
}