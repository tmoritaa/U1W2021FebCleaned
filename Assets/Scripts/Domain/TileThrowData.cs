using System.Collections.Generic;
using UnityEngine;

namespace Domain {
  public readonly struct TileThrowData {
    public readonly IEnumerable<Piece> PiecesToAdd;
    public readonly Vector2Int Coord;

    public TileThrowData(IEnumerable<Piece> piecesToAdd, Vector2Int coord) {
      this.PiecesToAdd = piecesToAdd;
      this.Coord = coord;
    }
  }
}