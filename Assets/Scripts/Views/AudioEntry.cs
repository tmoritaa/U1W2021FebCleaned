using System;
using UnityEngine;

namespace Views {
  [Serializable]
  public class AudioEntry {
    [SerializeField] private string id;
    [SerializeField] private AudioSource audio;

    public string Id => this.id;
    public AudioSource Audio => this.audio;
  }
}