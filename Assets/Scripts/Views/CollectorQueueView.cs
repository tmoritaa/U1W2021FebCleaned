using System.Collections.Generic;
using Domain;
using UnityEngine;

namespace Views {
  public class CollectorQueueView : MonoBehaviour {
    [SerializeField] private List<Transform> placementPoints;

    private Queue<ICollectorPresenter> shownCollectors = new Queue<ICollectorPresenter>();

    public void EnqueueCollector(ICollectorPresenter collector) {
      var placementPt = this.placementPoints[this.shownCollectors.Count];

      collector.UpdateForQueueDisplay(Vector2.zero, placementPt);

      this.shownCollectors.Enqueue(collector);
    }

    public void DequeueCollector() {
      var frontCollector = this.shownCollectors.Dequeue();
      frontCollector.Hide();

      int count = 0;
      foreach (var shownCollector in this.shownCollectors) {
        var placementPt = this.placementPoints[count];
        shownCollector.UpdateForQueueDisplay(Vector2.zero, placementPt);
        count += 1;
      }
    }
  }
}
