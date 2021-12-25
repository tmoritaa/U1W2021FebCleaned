using System.Collections.Generic;
using Domain;
using Presenters;
using Views;

namespace Applications {
  public class GameContext {
    private readonly PieceGenerator pieceGenerator;
    private readonly CollectorFacadeFactory collectorFacadeFactory;
    private readonly ICollectorQueuePresenter collectorQueuePresenter;

    public readonly Queue<CollectorFacade> CollectorBag = new Queue<CollectorFacade>();

    public GameContext(PieceGenerator pieceGenerator, CollectorFacadeFactory collectorFacadeFactory, ICollectorQueuePresenter collectorQueuePresenter) {
      this.pieceGenerator = pieceGenerator;
      this.collectorFacadeFactory = collectorFacadeFactory;
      this.collectorQueuePresenter = collectorQueuePresenter;
    }

    public string NextScene { get; private set; }

    public void AddNewCollectorToQueue() {
      var pieces = new List<Piece>();

      for (int j = 0; j < 3; ++j) {
        pieces.Add(this.pieceGenerator.GeneratePiece());
      }
      var collector = this.collectorFacadeFactory.Create(pieces);
      collector.Initialize();
      this.CollectorBag.Enqueue(collector);

      this.collectorQueuePresenter.EnqueueCollectorDisplay(collector.presenter);
    }

    public void SetNextScene(string scene) {
      this.NextScene = scene;
    }
  }
}