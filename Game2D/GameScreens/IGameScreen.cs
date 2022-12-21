namespace Game2D.GameScreens
{
    public interface IGameScreen
    {
        void Initialize();

        void Update(float deltaTime);

        void Draw(float deltaTime);
    }
}