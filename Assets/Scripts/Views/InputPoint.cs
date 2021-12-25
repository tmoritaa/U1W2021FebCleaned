using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Views;

public class InputPoint : MonoBehaviour {
  [SerializeField] private SpriteRenderer arrowSprite;
  [SerializeField] private SpriteRenderer shadowSprite;

  private readonly Subject<InputData> inputSubject = new Subject<InputData>();

  private Vector2Int coord;
  private Vector2Int facingDir;

  public IObservable<InputData> OnInput => this.inputSubject;

  private bool disabledDueToRun = false;
  private bool disabledDueToMass = false;

  private bool IsClickable => !this.disabledDueToMass && !this.disabledDueToRun;

  public void Initialize(Vector2Int coord, Vector2Int facingDir) {
    this.coord = coord;
    this.facingDir = facingDir;

    this.transform.Rotate(new Vector3(0, 0, 1), 45);
    if (this.facingDir == Vector2Int.down) {
      this.transform.Rotate(new Vector3(0, 0, 1), 90);
      var scale = this.transform.localScale;
      scale.y *= -1;
      this.transform.localScale = scale;
    } else if (this.facingDir == Vector2Int.left) {
      this.transform.Rotate(new Vector3(0, 0, 1), 180);
      var scale = this.transform.localScale;
      scale.y *= -1;
      this.transform.localScale = scale;
    } else if (this.facingDir == Vector2Int.up) {
      this.transform.Rotate(new Vector3(0, 0, 1), 270);
    }
  }

  public void ToggleDisableDueToRun(bool b) {
    bool prevClickable = this.IsClickable;

    this.disabledDueToRun = b;

    if (prevClickable != this.IsClickable) {
      this.UpdateEnabilityDisplay(this.IsClickable);
    }
  }

  public void ToggleDisableDueToMass(bool b) {
    bool prevClickable = this.IsClickable;

    this.disabledDueToMass = b;

    if (prevClickable != this.IsClickable) {
      this.UpdateEnabilityDisplay(this.IsClickable);
    }
  }

  private void UpdateEnabilityDisplay(bool b) {
    this.ChangeAlphaOfArrow(b ? 1f : 0.5f);
    this.shadowSprite.gameObject.SetActive(b);
  }

  private void ChangeAlphaOfArrow(float alpha) {
    var curcolor = this.arrowSprite.color;
    curcolor.a = alpha;
    this.arrowSprite.color = curcolor;
  }

  private void OnMouseDown() {
    if (this.IsClickable) {
      this.inputSubject.OnNext(new InputData(this.coord, this.facingDir));
    }
  }

  private void OnDestroy() {
    this.inputSubject.Dispose();
  }
}