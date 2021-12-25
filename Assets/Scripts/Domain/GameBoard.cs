using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Domain {
  public class GameBoard : IEnumerable<Tile> {
    private readonly Tile[] tiles;

    public Tile this[int i] => this.tiles[i];

    public Tile this[int x, int y] => this.tiles[y * this.Width + x];

    public Tile this[Vector2Int coord] => this[coord.x, coord.y];

    public GameBoard(int width, int height) {
      this.Width = width;
      this.Height = height;
      this.tiles = new Tile[width * height];

      for (int i = 0; i < this.Width * this.Height; ++i) {
        var coord = new Vector2Int(i % this.Width, i / this.Height);
        this.tiles[i] = new Tile(coord);
      }
    }

    public int Width { get; }
    public int Height { get; }

    public IEnumerator<Tile> GetEnumerator() {
      return ((IEnumerable<Tile>)this.tiles).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this.tiles.GetEnumerator();
    }
  }
}