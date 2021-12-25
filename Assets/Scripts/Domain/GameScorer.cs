using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Domain {
  public class GameScorer {
    private const int NUM_PIECE_TYPES_FOR_FAILED_MATCH = 3;
    private const int MAX_COMBO = 5;

    private enum MatchType {
      FiveOfAKind,
      FullHouse,
      FourOfAKind,
      TwoPairs,
      ThreeOfAKind,
      OnePair,
      FailedMatch,
    }

    public int Score { get; private set; } = 0;
    public int ComboCount { get; private set; } = 1;

    public int CalculateBaseScore(Tile tile) {
      if (tile.Pieces.Count != 5 && tile.Pieces.Count != 4 && tile.Pieces.Count != 3) {
        Debug.Log($"Tile {tile.Coord} number is {tile.Pieces.Count}. Should never happen");
      }

      var matchType = this.DetermineMatchType(tile.Pieces);

      return this.GetBaseScore(matchType);
    }

    public void ResetCombo() {
      this.ComboCount = 1;
    }

    public void IncrementCombo() {
      this.ComboCount = Math.Min(this.ComboCount + 1, MAX_COMBO);
    }

    public void DecrementCombo() {
      this.ComboCount = Math.Max(this.ComboCount - 1, 1);
    }

    public void AddScore(int score) {
      this.Score = Math.Max(this.Score + score, 0);
    }

    private MatchType DetermineMatchType(IReadOnlyList<Piece> pieces) {
      var numPieces = new Dictionary<Piece, int>();

      foreach (var piece in pieces) {
        if (!numPieces.ContainsKey(piece)) {
          numPieces[piece] = 0;
        }

        numPieces[piece] += 1;
      }

      if (numPieces.Count >= NUM_PIECE_TYPES_FOR_FAILED_MATCH) {
        return MatchType.FailedMatch;
      }

      if (numPieces.Count(kvp => kvp.Value == 5) > 0) {
        return MatchType.FiveOfAKind;
      } else if (numPieces.Count(kvp => kvp.Value == 3) >= 1 && numPieces.Count(kvp => kvp.Value == 2) >= 2) {
        return MatchType.FullHouse;
      } else if (numPieces.Count(kvp => kvp.Value == 4) > 0) {
        return MatchType.FourOfAKind;
      } else if (numPieces.Count(kvp => kvp.Value == 2) >= 2) {
        return MatchType.TwoPairs;
      } else if (numPieces.Count(kvp => kvp.Value == 3) >= 1) {
        return MatchType.ThreeOfAKind;
      } else if (numPieces.Count(kvp => kvp.Value == 2) >= 1) {
        return MatchType.OnePair;
      }

      return MatchType.FailedMatch;
    }

    private int GetBaseScore(MatchType matchType) {
      switch (matchType) {
        case MatchType.OnePair:
          return 100;
        case MatchType.TwoPairs:
          return 200;
        case MatchType.ThreeOfAKind:
          return 300;
        case MatchType.FourOfAKind:
          return 1000;
        case MatchType.FullHouse:
          return 500;
        case MatchType.FiveOfAKind:
          return 2000;
        case MatchType.FailedMatch:
          return -200;
      }

      return 0;
    }
  }
}