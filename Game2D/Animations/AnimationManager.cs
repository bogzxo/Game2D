using OpenTK.Mathematics;
using System.Drawing;

namespace Game2D.Animations
{
    public class AnimationManager
    {
        #region Private Members

        private float timer;
        private string currentAnimation;
        private Dictionary<string, AnimationInformation> animations;

        #endregion Private Members

        #region Public Members

        public Vector2 SpriteSize { get; }
        public float AnimationDelay { get; set; } = 0.1f;
        public bool RequiresSpriteUpdate { get; private set; }

        #endregion Public Members

        public AnimationManager(Vector2 spriteSize)
        {
            SpriteSize = spriteSize;

            animations = new Dictionary<string, AnimationInformation>();
        }

        public void Update(float dt)
        {
            if (currentAnimation.Equals(string.Empty)) return;

            timer += dt;

            if (timer > AnimationDelay)
            {
                RequiresSpriteUpdate = true;
                animations[currentAnimation].ProgressFrame();
                timer = 0;
            } else RequiresSpriteUpdate = false;
        }

        public void SetCurrentAnimation(string name)
        {
            if (!animations.ContainsKey(name)) throw new Exception($"Animation '{name}' does not exist!");
            currentAnimation = name;
        }

        public (RectangleF boundingBox, AnimationInformation info) GetCurrentAnimation()
        {
            var anim = animations[currentAnimation];

            RectangleF boudingBox = new RectangleF(anim.Position.X + anim.CurrentFrame, anim.Position.Y,
                SpriteSize.X, SpriteSize.Y);

            return (boudingBox, anim);
        }

        public void AddAnimation(string name, AnimationInformation info)
        {
            if (animations.ContainsKey(name)) { throw new Exception($"Attempted to add existing animation {name}"); }

            animations.Add(name, info);
        }
    }
}