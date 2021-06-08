using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class FreezeEnemies : PowerUp
    {
        public FreezeEnemies(int x, int y, Player p, float duration, Bitmap icon, EntitiesGroup targets) : base(x, y, p, duration, icon, targets)
        { }

        public override void RevertEffect()
        {
            foreach (GameObject obj in Targets.EntitiesList)
            {
                obj.Hostility = ObjectType.hostile;
            }
        }

        public override void TriggerEffect()
        {
            base.TriggerEffect();
            if (!EffectTimedOut())
            {
                foreach (GameObject obj in Targets.EntitiesList)
                {
                    obj.Hostility = ObjectType.frozen;
                }
            }
        }
    }
}
