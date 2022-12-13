using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.GameScreens
{
    public interface IGameScreen
    {
        void Initialize();
        void Update(float deltaTime);
        void Draw(float deltaTime);
    }
}
