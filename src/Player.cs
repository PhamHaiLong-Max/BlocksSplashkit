using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class Player : MovableGameObject
    {
        private bool _charged = true;
        private bool _charging = false;
        private Gun _gun;
        private int _killCount = 0;
        Timer _skillRechargeTimer = new Timer();
        Timer _regenTimer = new Timer();

        public Player() : base(Color.Black, 20, 480, 300)
        {
            Weapon = new Gun();
            Speed = 2;
            _regenTimer.Start();
        }

        public Gun Weapon
        {
            get
            {
                return _gun;
            }
            set
            {
                _gun = value;
            }
        }

        public bool SkillCharged
        {
            get
            {
                return _charged;
            }
            set
            {
                _charged = value;
            }
        }

        public bool SkillState
        {
            get
            {
                return _charging;
            }
            set
            {
                _charging = value;
            }
        }

        public int Kill
        {
            get
            {
                return _killCount;
            }
            set
            {
                _killCount = value;
            }
        }

        public void DisplayPlayerDetails(Font optimusFont)
        {
            //HP gauge
            SwinGame.FillRectangle(Color.Red, 300, 25, 400, 50);
            //current HP
            SwinGame.FillRectangle(Color.Green, 300, 25, (400 / BaseHP) * HP, 50);
            //HP text
            if(HP < 10)
                SwinGame.DrawText("0" + HP.ToString(), Color.White, optimusFont, 150, 30);
            else
                SwinGame.DrawText(HP.ToString(), Color.White, optimusFont, 150, 30);
            SwinGame.DrawText("/" + BaseHP.ToString() , Color.White, optimusFont, 190, 30);
            SwinGame.DrawText("HP", Color.White, optimusFont, 250, 30);
            //kill counter
            SwinGame.DrawText("Kills: " + _killCount.ToString(), Color.White, optimusFont, 750, 80);
            //ammo
            if(Weapon.Reloading == false)
                SwinGame.DrawText("Ammo: " + Weapon.Round.ToString() + "/" + Weapon.RoundCapacity.ToString(), Color.White, optimusFont, 150, 80);
            else
                SwinGame.DrawText("Reloading", Color.White, optimusFont, 150, 80);
            //skill cooldown
            if (!SkillCharged)
                SwinGame.DrawText("Skill: " + ((1500 - _skillRechargeTimer.Ticks) / 100).ToString(), Color.White, optimusFont, 450, 80);
            if (SkillCharged == false && SkillState == false)
            {
                SkillState = true;
                _skillRechargeTimer.Start();
            }
            if (_skillRechargeTimer.Ticks >= 1500 && SkillCharged == false && SkillState == true)
            {
                _skillRechargeTimer.Stop();
                RechargeSkill();
                SkillState = false;
                _skillRechargeTimer.Start();
            }
            if(SkillCharged)
                SwinGame.DrawText("Skill: READY", Color.White, optimusFont, 450, 80);
        }

        public void Regenerate()
        {
            //player HP regen (every 5 sec restores 1HP out of 20HP)
            if (_regenTimer.Ticks >= 5000 && HP > 0)
            {
                if (HP < BaseHP)
                    HP++;
                _regenTimer.Stop();
                _regenTimer.Start();
            }
        }

        public void RechargeSkill()
        {
            SkillCharged = true;
        }

        public override void DisplayItself()
        {
            base.DisplayItself();
            //displaying the range of the teleportation skill
            if(SkillCharged)
                SwinGame.DrawRectangle(Color.Green, ModX-110, ModY-110, 260, 260);
            else
                SwinGame.DrawRectangle(Color.DarkOrange, ModX - 110, ModY - 110, 260, 260);
        }
        
        public void Teleport(Direction direction)
        {
            if (SkillCharged == true)
            {
                SkillCharged = false;
                switch (direction)
                {
                    case Direction.up:
                        if (ModY < 300)
                            ModY = 150;
                        else
                            ModY -= 150;
                        break;
                    case Direction.left:
                        if (ModX < 150)
                            ModX = 0;
                        else
                            ModX -= 150;
                        break;
                    case Direction.down:
                        if (ModY >= 460)
                            ModY = 610;
                        else
                            ModY += 150;
                        break;
                    case Direction.right:
                        if (ModX > 810)
                            ModX = 960;
                        else
                            ModX += 150;
                        break;
                }
            }
        }
    }
}
