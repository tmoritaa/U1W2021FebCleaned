using Domain;

namespace Applications {
  public interface ICollectorQueuePresenter {
    void EnqueueCollectorDisplay(ICollectorPresenter collector);
    void DequeueCollectorDisplay();
  }
}