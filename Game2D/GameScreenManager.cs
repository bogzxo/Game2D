using Game2D.GameScreens;

namespace Game2D
{

    public class GameScreenManager
    {
        struct GameScreenManagerGameScreenInformation
        {
            public static GameScreenManagerGameScreenInformation Default
                = new();

            public bool HasBeenInitialized { get; set; }
        }

        private Dictionary<Type, (IGameScreen screen, GameScreenManagerGameScreenInformation information)> gameScreens;
        private (IGameScreen screen, GameScreenManagerGameScreenInformation information) currentGameScreen;

        public IGameScreen? GetGameScreen() => currentGameScreen.screen;

        public GameScreenManager()
        {
            gameScreens = new Dictionary<Type, (IGameScreen screen, GameScreenManagerGameScreenInformation information)>();
            SetGameScreen(typeof(GameScreens.MainMenuGameScreen));
        }

        public void Update(float dt)
        {
            if (!currentGameScreen.information.HasBeenInitialized)
            {
                currentGameScreen.information.HasBeenInitialized = true;
                currentGameScreen.screen.Initialize();
            }
            currentGameScreen.screen.Update(dt);
        }
        public void Draw(float dt) => currentGameScreen.screen.Draw(dt);
        public void SetGameScreen(Type type)
        {
            if (!gameScreens.ContainsKey(type))
            {
                if (Activator.CreateInstance(type) is not IGameScreen gameScreen)
                    throw new Exception($"Activator was unable to instantiate GameScreen with type {type}");

                gameScreens.Add(type, (gameScreen, GameScreenManagerGameScreenInformation.Default));
            }
            currentGameScreen = gameScreens[type];
        }
    }
}
