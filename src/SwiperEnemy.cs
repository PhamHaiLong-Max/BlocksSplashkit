using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class SwiperEnemy : Enemy
    {
        private Direction _dir = Direction.down;

        public SwiperEnemy(int hp, Player p) : base (hp, 0, 0, p)
        {
            Width = 1100;
            Height = 14;
        }

        public override void MoveDown()
        {
            ModY += Speed;
        }

        public override void MoveUp()
        {
            ModY -= Speed;
        }

        public override void SpecialMove()
        {
            if (_dir == Direction.down)
                MoveDown();
            else
                MoveUp();
            if (ModY > 670)
                _dir = Direction.up;
            if (ModY < 120)
                _dir = Direction.down;
        }

        public override void DisplayItself()
        {
            if (SpawnTimer.Ticks > 3000)
            {
                SpawnTimer.Pause();
                base.DisplayItself();
                Hostility = ObjectType.hostile;
                Speed = 2;
            }
            else
            {
                SwinGame.FillRectangle(Color.Orange, ModX, ModY, Width, Height);
            }
        }
    }
}
