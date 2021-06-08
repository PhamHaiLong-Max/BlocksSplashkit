using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class NormalEnemy : Enemy, IDetectBullets
    {
        public NormalEnemy(int hp, int x, int y, Player p) : base(hp, x, y, p)
        { }

        public override void SpecialMove()
        {
            if (ModX < p.ModX)
            {
                MoveRight();
            }
            if (ModX > p.ModX)
            {
                MoveLeft();
            }
            if (ModY < p.ModY)
            {
                MoveDown();
            }
            if (ModY > p.ModY)
            {
                MoveUp();
            }
        }

        public override void DisplayItself()
        {
            if (SpawnTimer.Ticks > 3000)
            {
                if (Hostility == ObjectType.neutral)
                    Hostility = ObjectType.hostile;
                Speed = 1;
                SpawnTimer.Pause();
                base.DisplayItself();
                UpdateHPColor();
            }
            else
            {
                SwinGame.FillRectangle(Color.Orange, ModX, ModY, 40, 40);
            }
        }

        public void DetectBullets(EntitiesGroup bulletEntities)
        {
            if (Hostility != ObjectType.neutral)
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
                            bullet.TakeDamage(bullet.HP);
                        }
                    }
                    //hitbox scan in case the bullet is shooting horizontally
                    if (bullet.FlyingDirection == Direction.left || bullet.FlyingDirection == Direction.right)
                    {
                        if ((ModX <= bullet.ModX + 45) && (ModX + 40 >= bullet.ModX) && (ModY + Height >= bullet.ModY) && (ModY <= bullet.ModY + 4))
                        {
                            TakeDamage(bullet.HP);
                            bullet.TakeDamage(bullet.HP);
                        }
                    }
                }
            }
        }

        //public void Repositioning(EnemyHorde enemies)
        //{
        //    foreach(Enemy e in enemies.EnemyList)
        //    {
        //        if(e != this)
        //        {
        //            //moving to the right into another enemy
        //            if (ModX + Width > e.ModX && ModX + Width <= e.ModX + Speed * 2 && ModY + Height > e.ModY && ModY - Height < e.ModY)
        //                MoveLeft();
        //            //moving to the left into another enemy
        //            if (ModX - Width < e.ModX && ModX + Width >= e.ModX - Speed * 2 && ModY + Height > e.ModY && ModY - Height < e.ModY)
        //                MoveRight();
        //        }
        //    }
        //}

        //color represents the healthiness of an enemy
        public void UpdateHPColor()
        {
            if (HP <= BaseHP / 5 * 4)
                Color = Color.OrangeRed;
            if (HP <= BaseHP / 5 * 3)
                Color = Color.Orange;
            if (HP <= BaseHP / 5 * 2)
                Color = Color.Yellow;
            if (HP <= BaseHP / 5)
                Color = Color.LightYellow;
        }
    }
}
