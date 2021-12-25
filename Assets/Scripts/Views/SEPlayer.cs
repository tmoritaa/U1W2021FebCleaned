using System.Collections.Generic;
using UnityEngine;

namespace Views {
  public class SEPlayer : MonoBehaviour {
    [SerializeField] private List<AudioEntry> entries;

    private Dictionary<string, AudioSource> entryDict = new Dictionary<string, AudioSource>();

    private HashSet<string> audioPlayRequests = new HashSet<string>();

    void Awake() {
      foreach (var entry in this.entries) {
        this.entryDict.Add(entry.Id, entry.Audio);
      }
    }

    private void LateUpdate() {
      foreach (var playRequest in this.audioPlayRequests) {
        var audio = this.entryDict[playRequest];
        audio.Play();
      }

      this.audioPlayRequests.Clear();
    }

    public void RequestPlayAudio(string id) {
      this.audioPlayRequests.Add(id);
    }
  }
}