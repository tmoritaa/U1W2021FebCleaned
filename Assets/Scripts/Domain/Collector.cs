using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domain {
  public class Collector {
    private readonly float velocityPerSec;
    private readonly List<Piece> heldPieces = new List<Piece>();

    public Collector(IEnumerable<Piece> startingPieces, float velocityPerSec) {
      this.heldPieces.AddRange(startingPieces);
      this.velocityPerSec = velocityPerSec;
    }

    public Vector2Int MoveDir { get; private set; }
    public Vector2Int CurCoord { get; private set; }
    public Vector2Int NextCoord { get; private set; }
    public float TravelPercent { get; private set; }
    public IReadOnlyList<Piece> HeldPieces => this.heldPieces;

    public bool HasReachedNextCoord => this.TravelPercent >= 1.0f;

    public void PrepareForStart(Vector2Int curCoord, Vector2Int facingDir) {
      this.CurCoord = curCoord;
      this.MoveDir = facingDir;
    }

    public void SetupForNextMove() {
      this.NextCoord = this.CurCoord + this.MoveDir;
      this.TravelPercent = 0;
    }

    public void MoveTowardsNextCoord(float timeStep) {
      this.TravelPercent += timeStep * this.velocityPerSec;

      if (this.HasReachedNextCoord) {
        this.CurCoord = this.NextCoord;
      }
    }

    public void AddPieces(IReadOnlyList<Piece> pieces) {
      this.heldPieces.AddRange(pieces);
    }

    public void ClearPieces() {
      this.heldPieces.Clear();
    }
  }
}