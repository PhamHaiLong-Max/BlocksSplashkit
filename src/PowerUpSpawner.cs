using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class PowerUpSpawner
    {
        private List<PowerUp> _powerUps = new List<PowerUp>();
        private Timer _spawnTimer = new Timer();
        private Player _p;
        private Bitmap[] _icons;
        private EntitiesGroup _targets;
        private Random _seed = new Random();

        public PowerUpSpawner(Player p, Bitmap[] icons, EntitiesGroup target)
        {
            _p = p;
            _spawnTimer.Start();
            _icons = icons;
            _targets = target;
        }

        public List<PowerUp> PowerUpsList
        {
            get
            {
                return _powerUps;
            }
        }

        public void SpawnPowerUps(EntitiesGroup entities)
        {
            foreach(PowerUp pwr in _powerUps.ToList())
            {
                if (pwr.EffectTimedOut())
                {
                    _powerUps.Remove(pwr);
                    _targets.EntitiesList.Remove(pwr);
                }
            }
            if(_spawnTimer.Ticks > 7500 && _powerUps.Count <= 5)
            {
                int x, y;
                PowerUp pwr;
                x = _seed.Next(10, 951);
                y = _seed.Next(160, 551);
                if (x % 2 == 1)
                    pwr = new FreezeEnemies(x, y, _p, 2, _icons[0], _targets);
                else
                    pwr = new SpeedBoost(x, y, _p, 4, _icons[1], _targets);
                _powerUps.Add(pwr);
                entities.AddObject(pwr);
                _spawnTimer.Reset();
            }
        }
    }
}
