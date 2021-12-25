using Applications;
using Applications.States;
using Domain;
using Presenters;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Views;

public class GameLifetimeScope : LifetimeScope {
  [SerializeField] private CollectorView collectorPrefab;
  [SerializeField] private Transform collectorRoot;
  [SerializeField] private BoardView boardView;
  [SerializeField] private ScoreView scoreView;
  [SerializeField] private TimerView timerView;
  [SerializeField] private CollectorQueueView collectorQueueView;
  [SerializeField] private ResultModal resultModal;
  [SerializeField] private IntroOutroPresenter introOutroPresenter;
  [SerializeField] private SEPlayer sePlayer;

  [SerializeField] private Image fadeImage;

  [SerializeField] private int gameLength = 180;

  protected override void Configure(IContainerBuilder builder) {
    Vector2Int boardDim = new Vector2Int(5, 5);

    builder.RegisterInstance<IntroOutroPresenter>(this.introOutroPresenter);
    builder.RegisterInstance<SEPlayer>(this.sePlayer);

    builder.Register<GameTimer>(Lifetime.Singleton)
      .AsSelf()
      .AsImplementedInterfaces()
      .WithParameter<int>(this.gameLength);

    builder.Register<GameBoardFacade>(Lifetime.Singleton)
      .AsSelf()
      .AsImplementedInterfaces()
      .WithParameter<GameBoard>(new GameBoard(boardDim.x, boardDim.y));
    builder.Register<BoardPresenter>(Lifetime.Singleton)
      .AsImplementedInterfaces();
    builder.RegisterInstance<BoardView>(this.boardView);

    builder.Register<GameScorer>(Lifetime.Singleton);
    builder.Register<ScorePresenter>(Lifetime.Singleton).AsImplementedInterfaces();
    builder.RegisterInstance<ScoreView>(this.scoreView);

    builder.Register<PosTranslator>(Lifetime.Singleton)
      .AsSelf()
      .WithParameter<Vector2Int>(boardDim);

    builder.Register<CollectorFacadeFactory>(Lifetime.Singleton)
      .WithParameter<CollectorView>(this.collectorPrefab)
      .WithParameter<Transform>(this.collectorRoot)
      .WithParameter<float>(5f);

    builder.Register<PieceGenerator>(Lifetime.Singleton);

    builder.Register<TimerPresenter>(Lifetime.Singleton)
      .AsImplementedInterfaces();
    builder.RegisterInstance<TimerView>(this.timerView)
      .AsSelf();

    builder.Register<CollectorQueuePresenter>(Lifetime.Singleton)
      .AsImplementedInterfaces();
    builder.RegisterInstance<CollectorQueueView>(this.collectorQueueView);

    builder.Register<ResultPresenter>(Lifetime.Singleton)
      .AsImplementedInterfaces();
    builder.RegisterInstance<ResultModal>(this.resultModal);

    builder.Register<GameContext>(Lifetime.Singleton);

    builder.Register<GameSetupState>(Lifetime.Scoped);
    builder.Register<GameIntroState>(Lifetime.Scoped)
      .WithParameter<Image>(this.fadeImage);
    builder.Register<GameGameplayState>(Lifetime.Scoped);
    builder.Register<GameResultState>(Lifetime.Scoped);
    builder.Register<GameLeaveState>(Lifetime.Scoped)
      .WithParameter<Image>(this.fadeImage);
    builder.Register<GameStateMachine>(Lifetime.Singleton)
      .AsImplementedInterfaces();
  }
}