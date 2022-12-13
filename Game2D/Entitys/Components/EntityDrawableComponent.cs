using Game2D.GameScreens.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Entitys.Components
{
    public class EntityDrawableComponent : IEntityComponent
    {
        public bool Enabled { get; set; } = true;
        public Sprite? Sprite { get; set; }

        public EntityPhysicsComponent EntityPhysicsComponent { get; private set; }

        public EntityDrawableComponent(EntityPhysicsComponent physicsComponent)
        {
            EntityPhysicsComponent = physicsComponent;
        }

        public void Draw(float dt)
        {
            if (Enabled) Sprite?.Draw(dt);
        }


        public void Update(float dt)
        {
            if (Sprite == null || !Enabled) return;

            Sprite.Position = EntityPhysicsComponent.Position;
            Sprite.Update(dt);
        }
    }
}
