using System;
using UniRx;

namespace Domain {
  public class GameTimer : IDisposable {
    private readonly Subject<(int, int)> timeUpdateSubject = new Subject<(int, int)>();

    public IObservable<(int, int)> OnTimeUpdate => this.timeUpdateSubject;

    public GameTimer(int startingTime) {
      this.CurTime = startingTime;
    }

    public int CurTime { get; private set; }

    public void SubtractTime(int value) {
      this.CurTime = Math.Max(this.CurTime - value, 0);

      this.timeUpdateSubject.OnNext((this.CurTime, value));
    }

    public void Dispose() {
      this.timeUpdateSubject?.Dispose();
    }
  }
}