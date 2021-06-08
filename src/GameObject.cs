using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public abstract class GameObject
    {
        private Color _color;
        private int _x, _y, _width = 40, _height = 40;
        private ObjectType _hostility = ObjectType.neutral;

        public GameObject(Color color, int x, int y)
        {
            Color = color;
            ModX = x;
            ModY = y;
        }

        public ObjectType Hostility
        {
            get
            {
                return _hostility;
            }
            set
            {
                _hostility = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        
        public int ModX
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public int ModY
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }


        //cordinate is from the outliner
        public virtual void DisplayItself()
        {
            SwinGame.FillRectangle(Color.Black, ModX, ModY, Width, Height);
            SwinGame.FillRectangle(Color, ModX + 1, ModY + 1, Width - 2, Height - 2);
        }

        public bool PlayerCollision(Player p)
        {
            if (Hostility == ObjectType.hostile )
            {
                //if collide from the left
                if (ModX <= p.ModX)
                {
                    //from top or from bottom
                    if (p.ModX - ModX < Width && ((ModY <= p.ModY && p.ModY - ModY < Height) || (ModY >= p.ModY && ModY - p.ModY < p.Height)))
                        return true;
                }
                //collide from the right
                else if (ModX >= p.ModX)
                {
                    if (ModX - p.ModX < p.Width && ((ModY <= p.ModY && p.ModY - ModY < Height) || (ModY >= p.ModY && ModY - p.ModY < p.Height)))
                        return true;
                }
            }
            return false;
        }
    }
}
