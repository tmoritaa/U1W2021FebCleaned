using UnityEngine;

namespace Views {
  public struct InputData {
    public readonly Vector2Int Coord;
    public readonly Vector2Int FacingDir;

    public InputData(Vector2Int coord, Vector2Int facingDir) {
      this.Coord = coord;
      this.FacingDir = facingDir;
    }

    public override string ToString() {
      return $"Coord:{this.Coord} FacingDir:{this.FacingDir}";
    }
  }
}