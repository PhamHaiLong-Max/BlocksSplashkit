using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class Gun
    {
        private bool _reloadStatus = false;
        private int _round = 25;
        private int _capacity;

        public Gun()
        {
            _capacity = _round;
        }

        public int RoundCapacity
        {
            get
            {
                return _capacity;
            }
        }

        public bool Reloading
        {
            get
            {
                return _reloadStatus;
            }
            set
            {
                _reloadStatus = value;
            }
        }

        public int Round
        {
            get
            {
                return _round;
            }
            set
            {
                _round = value;
            }
        }

        public void Reload()
        {
            Round = 25;
            _reloadStatus = false;
        }

        public Bullet Shoot(Direction dir, int x, int y, SoundEffect pewFX)
        {
            if (Round > 0 && _reloadStatus == false)
            {
                Round--;
                SwinGame.PlaySoundEffect(pewFX);
                Bullet bullet = new Bullet(x, y);
                bullet.FlyingDirection = dir;
                return bullet;
            }
            else
            {
                Bullet bullet = new Bullet(x, y);
                bullet.HP = 0;
                return bullet;
            }
        }
    }
}
