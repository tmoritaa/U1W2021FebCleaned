using System.Collections.Generic;
using Applications;
using Domain;
using Presenters;
using UnityEngine;

namespace Views {
  public class CollectorFacadeFactory {
    private readonly GameBoardFacade gameBoard;
    private readonly CollectorView collectorPrefab;
    private readonly PosTranslator posTranslator;
    private readonly float velocityPerSec;
    private readonly Transform collectorRoot;
    private readonly SEPlayer sePlayer;


    public CollectorFacadeFactory(GameBoardFacade gameBoard, CollectorView collectorPrefab, PosTranslator posTranslator, float velocityPerSec, Transform collectorRoot, SEPlayer sePlayer) {
      this.gameBoard = gameBoard;
      this.collectorPrefab = collectorPrefab;
      this.posTranslator = posTranslator;
      this.velocityPerSec = velocityPerSec;
      this.collectorRoot = collectorRoot;
      this.sePlayer = sePlayer;
    }

    public CollectorFacade Create(IEnumerable<Piece> startingPieces) {
      var collectorView = GameObject.Instantiate(this.collectorPrefab, this.collectorRoot);
      var collectorPresenter = new CollectorPresenter(collectorView, this.posTranslator, this.collectorRoot, this.sePlayer);
      var collector = new Collector(startingPieces, this.velocityPerSec);

      return new CollectorFacade(collector, collectorPresenter, this.gameBoard);
    }
  }
}