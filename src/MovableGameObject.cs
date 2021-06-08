using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public abstract class MovableGameObject : GameObject
    {
        private int _spd = 0, _health, _baseHP;

        public MovableGameObject(Color color, int hp, int x, int y) : base(color, x, y)
        {
            HP = hp;
            _baseHP = hp;
        }

        public int BaseHP
        {
            get
            {
                return _baseHP;
            }
        }

        public int HP
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public int Speed
        {
            get
            {
                return _spd;
            }
            set
            {
                _spd = value;
            }
        }

        //movement cannot be made if the object would go pass the screen border
        public virtual void MoveUp()
        {
            if (ModY > 150 && Hostility != ObjectType.frozen)
                ModY += -Speed;
        }

        public virtual void MoveDown()
        {
            if (ModY < 610 && Hostility != ObjectType.frozen)
                ModY += Speed;
        }

        public virtual void MoveLeft()
        {
            if (ModX > 0 && Hostility != ObjectType.frozen)
                ModX += -Speed;
        }

        public virtual void MoveRight()
        {
            if (ModX < 960 && Hostility != ObjectType.frozen)
                ModX += Speed;
        }

        //take damage
        public void TakeDamage(int damage)
        {
            HP += -damage;
            if (HP < 0)
                HP = 0;
        }
    }
}
