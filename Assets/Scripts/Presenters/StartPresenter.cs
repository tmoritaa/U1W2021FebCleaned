using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Views;
using Random = UnityEngine.Random;

namespace Presenters {
  public class StartPresenter : MonoBehaviour {
    [SerializeField] private Image transitionGradient;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider slider;

    [SerializeField] private List<CollectorView> collectorViews;

    private List<Vector3> startingPos = new List<Vector3>();
    private List<float> runSpeed = new List<float>();

    private bool isTransitioning = false;

    private void Awake() {
      float curVolume;
      this.masterMixer.GetFloat("Volume", out curVolume);

      var volume = Mathf.Clamp(curVolume, this.slider.minValue, this.slider.maxValue);
      this.slider.value = volume;
    }

    void Start() {
      foreach (var view in this.collectorViews) {
        view.TriggerRunAnim();
        this.startingPos.Add(view.transform.localPosition);

        this.runSpeed.Add(Random.Range(4f, 6.5f));
      }
    }

    void Update() {
      var idx = 0;
      foreach (var view in this.collectorViews) {
        var pos = view.transform.localPosition;
        pos.x -= this.runSpeed[idx] * Time.deltaTime;

        if (pos.x < -12f) {
          pos = this.startingPos[idx];
        }

        view.UpdatePos(pos);

        idx += 1;
      }
    }

    public void StartGame() {
      if (this.isTransitioning) {
        return;
      }

      this.isTransitioning = true;
      this.transitionGradient.DOFade(1f, 0.75f)
        .OnComplete(() => SceneManager.LoadScene("Game"));
    }

    public void OnAudioSliderAdjusted(float value) {
      if (value <= this.slider.minValue + 2) {
        value = -80f;
      }

      this.masterMixer.SetFloat("Volume", value);
    }
  }
}