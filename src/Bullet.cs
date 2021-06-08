using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class Bullet : MovableGameObject
    {
        private Direction _dir = Direction.none;

        //plus 18 to the cords so that the bullet will be at the middle of the player. base speed is 30 for best collision test and visibility
        public Bullet(int x, int y) : base (Color.Purple, 1, x+18, y+18)
        {
            Speed = 30;
        }

        public Direction FlyingDirection
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }

        public void Fly()
        {
            switch(FlyingDirection)
            {
                case Direction.none:
                    break;
                case Direction.up:
                    MoveUp();
                    break;
                case Direction.left:
                    MoveLeft();
                    break;
                case Direction.down:
                    MoveDown();
                    break;
                case Direction.right:
                    MoveRight();
                    break;
            }
        }

        //overriding the movement since the bullet take different shape for moving up/down and left/right
        //bullets will go into neutral state if it hits a wall, and situated next to the wall (so that it doesn't look like it goes through the wall)
        //bullets that are neutral will disappear in the next frame (this handler is in gamemain)
        public override void MoveUp()
        {
            Width = 5;
            Height = 50;
            if (ModY >= 100)
                ModY += -Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveDown()
        {
            Width = 5;
            Height = 50;
            if (ModY <= 700)
                ModY += Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveLeft()
        {
            Width = 50;
            Height = 5;
            if (ModX >= 0)
                ModX += -Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveRight()
        {
            Width = 50;
            Height = 5;
            if (ModX <= 1000)
                ModX += Speed;
            else
                TakeDamage(HP);
        }
    }
}
