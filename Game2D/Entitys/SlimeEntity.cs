using Game2D.Entitys.Components;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Entitys
{
    public class SlimeEntity : IEntity
    {
        public Dictionary<Type, IEntityComponent> Components { get; set; }
        public EntityPhysicsComponent PhysicsComponent { get; private set; }
        public EntityDrawableComponent DrawableComponent { get; private set; }

        public SlimeEntity()
        {
            PhysicsComponent = new EntityPhysicsComponent(new Vector2(0, 0), Vector2.Zero, 1.0f);
            DrawableComponent = new EntityDrawableComponent(PhysicsComponent);

            Components = new Dictionary<Type, IEntityComponent>()
            {
                { typeof(EntityPhysicsComponent), PhysicsComponent },
                { typeof(EntityDrawableComponent), DrawableComponent }
            };
        }
        public void Draw(float dt)
        {
            foreach (var entityPair in Components)
                entityPair.Value.Draw(dt);
        }

        public void Update(float dt)
        {
            PhysicsComponent.Position = GameManager.Instance.Player.Position;

            foreach (var entityPair in Components)
                entityPair.Value.Update(dt);
        }
    }
}
