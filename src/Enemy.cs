using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public abstract class Enemy : MovableGameObject
    {
        private Timer _spawnTimer= new Timer();
        private Player _p;

        public Enemy(int hp, int x, int y, Player p) : base(Color.Red, hp, 0, 0)
        {
            ModX = x;
            ModY = y;
            SpawnTimer.Start();
            _p = p;
        }

        public Player p
        {
            get
            {
                return _p;
            }
        }

        public Timer SpawnTimer
        {
            get
            {
                return _spawnTimer;
            }
        }
        
        public abstract void SpecialMove();
    }
}
