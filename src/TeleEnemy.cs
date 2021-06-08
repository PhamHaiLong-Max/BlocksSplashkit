using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class TeleEnemy : Enemy, IDetectBullets
    {
        private bool _cordLocked = false;
        private bool _idle = true;
        private Point2D _cordinate;
        private Timer _skillChargeTime = new Timer();
        private Timer _skillSpacing = new Timer();
        private Timer _skillCooldown = new Timer();
        private int _spacingMultiplier;

        public TeleEnemy(int hp, int x, int y, int multiplier, Player p) : base(hp, x, y, p)
        {
            _spacingMultiplier = multiplier;
            _skillChargeTime.Start();
            _skillSpacing.Start();
            HP = 10;
        }

        public override void DisplayItself()
        {
            SwinGame.FillRectangle(Color.LightSkyBlue, ModX, ModY, 40, 40);
            if (SpawnTimer.Ticks > 3000)
            {
                if(_idle)
                    Color = Color.DarkBlue;
                else
                    UpdateHPColor();
                SpawnTimer.Pause();
                base.DisplayItself();
                Hostility = ObjectType.hostile;
                Speed = 0;
            }
            else if(SpawnTimer.Ticks == 3000)
                _skillSpacing.Start();
            else
            {
                _skillSpacing.Start();
            }
        }

        public void DetectBullets(EntitiesGroup bulletEntities)
        {
            if (Hostility == ObjectType.hostile)
            {
                foreach (Bullet bullet in bulletEntities.EntitiesList)
                {
                    //hitbox scan in case the bullet is shooting vertically
                    if (bullet.FlyingDirection == Direction.up || bullet.FlyingDirection == Direction.down)
                    {
                        //i suck
                        if ((ModX <= bullet.ModX + 5) && (ModX + Width >= bullet.ModX) && (ModY <= bullet.ModY + 45) && (ModY + 45 >= bullet.ModY))
                        {
                            TakeDamage(bullet.HP);
                            //double damage if the enemy is charging skill
                            if(_skillChargeTime.Ticks < 2000 && _cordLocked == false)
                                bullet.TakeDamage(bullet.HP);
                            bullet.TakeDamage(bullet.HP);
                        }
                    }
                    //hitbox scan in case the bullet is shooting horizontally
                    if (bullet.FlyingDirection == Direction.left || bullet.FlyingDirection == Direction.right)
                    {
                        if ((ModX <= bullet.ModX + 45) && (ModX + 40 >= bullet.ModX) && (ModY + Height >= bullet.ModY) && (ModY <= bullet.ModY + 4) && !_idle)
                        {
                            TakeDamage(bullet.HP);
                            if (_skillChargeTime.Ticks < 2000 && _cordLocked == false)
                                bullet.TakeDamage(bullet.HP);
                            bullet.TakeDamage(bullet.HP);
                        }
                    }
                }
            }
        }

        public override void SpecialMove()
        {
            if (_skillSpacing.Ticks > _spacingMultiplier*500)
            {
                _skillSpacing.Stop();
                _idle = false;
                _skillChargeTime.Start();
            }
            if(!_idle)
            {
                if (_skillChargeTime.Ticks < 2000 && _cordLocked == false)
                {
                    Color = Color.DarkGray;
                }
                else if (_skillChargeTime.Ticks < 3000 && _cordLocked == false)
                {
                    TargetReticle(1);
                }
                else if (_skillChargeTime.Ticks < 4000 && _cordLocked == false)
                {
                    TargetReticle(2);
                }
                else if (_skillChargeTime.Ticks < 5000)
                {
                    if (_cordLocked == false)
                    {
                        _cordLocked = true;
                        _cordinate.X = p.ModX;
                        _cordinate.Y = p.ModY;
                    }
                    TargetReticle(3);
                }
                else if (_cordLocked == true && _skillChargeTime.Ticks > 4000)
                {
                    _cordLocked = false;
                    _skillCooldown.Stop();
                    _skillCooldown.Reset();
                    _skillCooldown.Start();
                    TargetReticle(4);
                }
                else
                    Color = Color.Red;
            }
        }

        //color represents the healthiness of an enemy
        public void UpdateHPColor()
        {
            if (HP <= BaseHP / 5 * 4)
                Color = Color.Purple;
            if (HP <= BaseHP / 5 * 3)
                Color = Color.ForestGreen;
            if (HP <= BaseHP / 5 * 2)
                Color = Color.LightGreen;
            if (HP <= BaseHP / 5)
                Color = Color.LightSkyBlue;
        }

        public void TargetReticle(int state)
        {
            switch(state)
            {
                case 1:
                    SwinGame.DrawRectangle(Color.DarkGreen, p.ModX - 6, p.ModY - 6, p.Width + 12, p.Height + 12);
                    break;
                case 2:
                    SwinGame.DrawRectangle(Color.Orange, p.ModX - 4, p.ModY - 4, p.Width + 8, p.Height + 8);
                    break;
                case 3:
                    SwinGame.DrawRectangle(Color.Red, _cordinate.X - 2, _cordinate.Y - 2, p.Width + 4, p.Height + 4);
                    break;
                case 4:
                    ModX = (int)_cordinate.X;
                    ModY = (int)_cordinate.Y;
                    _skillChargeTime.Stop();
                    _skillChargeTime.Reset();
                    _skillChargeTime.Start();
                    break;
            }
        }
    }
}
