using System.Collections.Generic;
using Domain;
using UnityEngine;

namespace Views {
  public class PieceView : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;

    public void UpdateAndShow(Piece piece) {
      this.gameObject.SetActive(true);
      this.spriteRenderer.sprite = this.sprites[piece.Id];
    }

    public void Hide() {
      this.gameObject.SetActive(false);
    }
  }
}