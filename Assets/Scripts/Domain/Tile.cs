using System.Collections.Generic;
using UnityEngine;

namespace Domain {
  public class Tile {
    public readonly Vector2Int Coord;
    public readonly Reservation Reservations;

    private readonly List<Piece> pieces = new List<Piece>();

    public Tile(Vector2Int coord) {
      this.Coord = coord;
      this.Reservations = new Reservation();
    }

    public IReadOnlyList<Piece> Pieces => this.pieces;

    public void AddPiece(Piece piece) {
      this.pieces.Add(piece);
    }

    public void ClearPieces() {
      this.pieces.Clear();
    }
  }
}