using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public abstract class PowerUp : GameObject
    {
        private float _duration;
        private Player _p;
        private Timer _effectTimer = new Timer();
        private Bitmap _icon;
        private EntitiesGroup _targets;

        public PowerUp(int x, int y, Player p, float duration, Bitmap icon, EntitiesGroup targets) : base(Color.DeepPink, x, y)
        {
            Hostility = ObjectType.hostile;
            _p = p;
            _duration = duration * 1000;
            _icon = icon;
            Targets = targets;
        }

        public Player p
        {
            get
            {
                return _p;
            }
        }

        public EntitiesGroup Targets { get => _targets; set => _targets = value; }

        public override void DisplayItself()
        {
            SwinGame.FillRectangle(Color, ModX, ModY, Width - 2, Height - 2);
            SwinGame.DrawBitmap(_icon, ModX + 1, ModY + 1);
            if (PlayerCollision(_p))
            {
                TriggerEffect();
                ModY = 0;
            }
        }

        public abstract void RevertEffect();

        public bool EffectTimedOut()
        {
            if (_effectTimer.Ticks > _duration)
                return true;
            else
                return false;
        }

        public virtual void TriggerEffect()
        {
            _effectTimer.Start();
        }
    }
}
