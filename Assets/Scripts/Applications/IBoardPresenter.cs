using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;
using Views;

namespace Applications {
  public interface IBoardPresenter {
    IObservable<InputData> OnInput { get; }

    IObservable<Unit> OnRestartInput { get; }

    void Initialize(GameBoard gameBoard);

    void UpdateTile(Tile tile);

    void ToggleInputPointDueToRun(bool b, Vector2Int coord);

    public void ToggleAllInputPoints(bool b);

    UniTask AnimateForScoring(Tile tile, int score, int comboCount, CancellationToken token);
  }
}