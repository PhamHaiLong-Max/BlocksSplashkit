using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class EnemyHorde
    {
        private List<Enemy> _enemies = new List<Enemy>();
        private int _enemyCount = 1;
        private Random _seed = new Random();
        private int _difficulty = 0;

        public EnemyHorde(int difficulty, Player p)
        {
            _difficulty = difficulty;
            NewWave(p);
        }

        public int Count
        {
            get
            {
                return _enemyCount;
            }
            set
            {
                _enemyCount = value;
            }
        }

        public List<Enemy> EnemyList
        {
            get
            {
                return _enemies;
            }
            set
            {
                _enemies = value;
            }
        }

        public void NewWave(Player p)
        {
            int x;
            int y;
            int i;
            int j = 0;
            EnemyList.Clear();
            Count = Count + _difficulty;
            i = Count;
            if (_difficulty == 3)
                _enemies.Add(new SwiperEnemy(999, p));
            if (i > 10)
            {
                //if (i > 20)
                //{
                    while (i >= 10)
                    {
                        i = i - 2;
                        j++;
                        do
                        {
                            x = _seed.Next(10, 951);
                            y = _seed.Next(160, 551);
                        } while (OverlapCheck(x, y));
                        _enemies.Add(new TeleEnemy(10, x, y, j, p));
                    }
                //}
            }
            for (j = i; j > 0; j--)
            {
                do
                {
                    x = _seed.Next(10, 951);
                    y = _seed.Next(160, 551);
                } while (OverlapCheck(x, y));
                _enemies.Add(new NormalEnemy(5, x, y, p));
            }
        }

        public bool OverlapCheck(int x, int y)
        {
            foreach (Enemy e in _enemies)
            {
                if (Math.Abs(x - e.ModX) <= 40 && Math.Abs(y - e.ModY) <= 40)
                    return true;
            }
            return false;
        }
    }
}
