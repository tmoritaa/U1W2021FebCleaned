using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;
using VContainer;

namespace Views {
  public class BoardView : MonoBehaviour {
    [SerializeField] private TileView tilePrefab;
    [SerializeField] private InputPoint inputPointPrefab;

    [SerializeField] private Transform tileRoot;
    [SerializeField] private Transform inputPointRoot;

    private PosTranslator posTranslator;
    private SEPlayer sePlayer;

    private readonly Dictionary<Vector2Int, TileView> tiles = new Dictionary<Vector2Int, TileView>();
    private readonly Dictionary<Vector2Int, InputPoint> inputPoints = new Dictionary<Vector2Int, InputPoint>();

    private readonly Subject<InputData> inputSubject = new Subject<InputData>();
    private readonly Subject<Unit> restartInputSubject = new Subject<Unit>();

    public IObservable<InputData> OnInput => this.inputSubject;
    public IObservable<Unit> OnRestartInput => this.restartInputSubject;

    [Inject]
    public void Hookup(PosTranslator posTranslator, SEPlayer sePlayer) {
      this.posTranslator = posTranslator;
      this.sePlayer = sePlayer;
    }

    public void Initialize(GameBoard gameBoard) {
      for (int y = 0; y < gameBoard.Height; ++y) {
        for (int x = 0; x < gameBoard.Width; ++x) {
          var coord = new Vector2Int(x, y);

          var tile = Instantiate(this.tilePrefab, this.tileRoot, false);
          tile.transform.localPosition = this.posTranslator.CalculatePosFromCoord(coord);

          tile.Initialize(gameBoard[x, y], this.sePlayer);
          this.tiles.Add(coord, tile);
        }
      }

      // Instantiate input points
      for (int y = -1; y < gameBoard.Height + 1; ++y) {
        for (int x = -1; x < gameBoard.Width + 1; ++x) {
          bool xInBounds = x >= 0 && x < gameBoard.Width;
          bool yInBounds = y >= 0 && y < gameBoard.Height;

          if ((xInBounds && yInBounds) || (!yInBounds && !xInBounds)) {
            continue;
          }

          var coord = new Vector2Int(x, y);
          var inputPt = Instantiate(this.inputPointPrefab, this.inputPointRoot, false);

          var facingDir = Vector2Int.zero;
          if (yInBounds) {
            facingDir = new Vector2Int(-Math.Sign(x), 0);
          } else { // This means y is in bounds
            facingDir = new Vector2Int(0, -Math.Sign(y));
          }

          inputPt.Initialize(coord, facingDir);
          inputPt.OnInput
            .Subscribe(data => this.inputSubject.OnNext(data))
            .AddTo(this.gameObject);

          inputPt.transform.localPosition = this.posTranslator.CalculatePosFromCoord(coord);

          this.inputPoints.Add(coord, inputPt);
        }
      }
    }

    public void UpdateTile(Tile tile) {
      var tileView = this.tiles[tile.Coord];
      tileView.UpdateView();
    }

    public async UniTask AnimateTileForScoring(Tile tile, int score, int comboCount, CancellationToken token) {
      var tileView = this.tiles[tile.Coord];
      await tileView.AnimateForScoring(score, comboCount, token);
    }

    public void ToggleInputPointDueToRun(bool b, Vector2Int coord) {
      var inputPt = this.inputPoints[coord];
      inputPt.ToggleDisableDueToRun(!b);
    }

    public void ToggleAllInputPoints(bool b) {
      foreach (var inputPt in this.inputPoints.Values) {
        inputPt.ToggleDisableDueToMass(!b);
      }
    }

    private void Update() {
      if (Input.GetKeyUp(KeyCode.R)) {
        this.restartInputSubject.OnNext(Unit.Default);
      }
    }

    private void OnDestroy() {
      this.inputSubject.Dispose();
      this.restartInputSubject.Dispose();
    }
  }
}