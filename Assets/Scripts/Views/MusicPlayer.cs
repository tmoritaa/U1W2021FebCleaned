using System.Collections.Generic;
using UnityEngine;

namespace Views {
  public class MusicPlayer : MonoBehaviour {
    private static bool initialized;

    public static MusicPlayer Instance;

    [SerializeField] private List<AudioEntry> audioEntries;

    private Dictionary<string, AudioSource> entries = new Dictionary<string, AudioSource>();

    void Awake() {
      if (initialized) {
        Destroy(this.gameObject);
        return;
      }

      foreach (var entry in this.audioEntries) {
        this.entries.Add(entry.Id, entry.Audio);
      }

      DontDestroyOnLoad(this.gameObject);
      Instance = this;

      initialized = true;
    }

    public void Play(string id) {
      this.entries[id].Play();
    }
  }
}