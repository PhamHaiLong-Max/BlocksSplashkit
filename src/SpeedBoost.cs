using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class SpeedBoost : PowerUp
    {
        public SpeedBoost(int x, int y, Player p, float duration, Bitmap icon, EntitiesGroup targets) : base(x, y, p, duration, icon, targets)
        { }

        public override void RevertEffect()
        {
            p.Speed = 2;
        }

        public override void TriggerEffect()
        {
            base.TriggerEffect();
            if (!EffectTimedOut())
            {
                p.Speed = 3;
            }
        }
    }
}
