using Applications;
using Domain;
using Views;

namespace Presenters {
  public class CollectorQueuePresenter : ICollectorQueuePresenter {
    private readonly CollectorQueueView collectorQueueView;

    public CollectorQueuePresenter(CollectorQueueView collectorQueueView) {
      this.collectorQueueView = collectorQueueView;
    }

    public void EnqueueCollectorDisplay(ICollectorPresenter collector) {
      this.collectorQueueView.EnqueueCollector(collector);
    }

    public void DequeueCollectorDisplay() {
      this.collectorQueueView.DequeueCollector();
    }
  }
}