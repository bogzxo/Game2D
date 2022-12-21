using OpenTK.Mathematics;

namespace Game2D.Animations
{
    public class AnimationInformation
    {
        public Vector2 Position { get; }
        public int Length { get; }
        public int CurrentFrame { get; internal set; }

        public AnimationInformation(Vector2 position, int length)
        {
            Position = position;
            this.Length = length;
        }

        public void ProgressFrame()
        {
            CurrentFrame++;
            if (CurrentFrame > Length) CurrentFrame = 0;
        }
    }
}