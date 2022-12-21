namespace Game2D.Entities.Components
{
    public class EntityAIController : IEntityComponent
    {
        public bool Enabled { get; set; } = true;
        public IEntity Entity { get; set; }

        public void Draw(float dt)
        {
        }

        public void Update(float dt)
        {
        }
    }
}