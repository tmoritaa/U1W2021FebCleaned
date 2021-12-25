using Domain;
using Extensions;
using UnityEngine;

namespace Views {
  public class PosTranslator {
    private readonly Vector2Int boardDim;

    public PosTranslator(Vector2Int boardDim) {
      this.boardDim = boardDim;
    }

    public Vector3 CalculatePosFromCoord(Vector2Int coord) {
      var centerCoord = new Vector2Int(this.boardDim.x / 2, this.boardDim.y / 2);
      var adjustedCoord = (Vector2)(coord - centerCoord);
      adjustedCoord.y *= -1;
      adjustedCoord = adjustedCoord.Rotate(45f);
      var z = (this.boardDim.x - 1 - coord.x) + (coord.y);

      return new Vector3(adjustedCoord.x, adjustedCoord.y, z * -0.1f);
    }
  }
}