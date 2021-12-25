using System.Collections.Generic;
using UnityEngine;

namespace Domain {
  public class PieceGenerator {
    public const int PIECE_TYPE_COUNT = 3;

    private readonly Queue<Piece> pieces = new Queue<Piece>();

    public Piece GeneratePiece() {
      if (pieces.Count == 0) {
        this.FillBag();
      }

      var piece = this.pieces.Dequeue();
      return piece;
    }

    private void FillBag() {
      var pieceList = new List<Piece>();

      for (int i = 0; i < PIECE_TYPE_COUNT; ++i) {
        for (int j = 0; j < 3; ++j) {
          pieceList.Add(new Piece(i));
        }
      }
      this.Shuffle(pieceList);

      foreach (var piece in pieceList) {
        this.pieces.Enqueue(piece);
      }
    }

    private void Shuffle<T>(List<T> list) {
      int n = list.Count;
      while (n > 1) {
        n--;
        int k = Random.Range(0, n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
  }
}