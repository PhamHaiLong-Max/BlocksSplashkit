using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Dummy
{
    public enum Direction { up, down, left, right, none }

    public enum ObjectType { neutral, hostile, frozen }



    public class Bullet : MovableGameObject
    {
        private Direction _dir = Direction.none;

        //plus 18 to the cords so that the bullet will be at the middle of the player. base speed is 30 for best collision test and visibility
        public Bullet(int x, int y) : base(Color.Purple, 1, x + 18, y + 18)
        {
            Speed = 30;
        }

        public Direction FlyingDirection
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }

        public void Fly()
        {
            switch (FlyingDirection)
            {
                case Direction.none:
                    break;
                case Direction.up:
                    MoveUp();
                    break;
                case Direction.left:
                    MoveLeft();
                    break;
                case Direction.down:
                    MoveDown();
                    break;
                case Direction.right:
                    MoveRight();
                    break;
            }
        }

        //overriding the movement since the bullet take different shape for moving up/down and left/right
        //bullets will go into neutral state if it hits a wall, and situated next to the wall (so that it doesn't look like it goes through the wall)
        //bullets that are neutral will disappear in the next frame (this handler is in gamemain)
        public override void MoveUp()
        {
            Width = 5;
            Height = 50;
            if (ModY >= 100)
                ModY += -Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveDown()
        {
            Width = 5;
            Height = 50;
            if (ModY <= 700)
                ModY += Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveLeft()
        {
            Width = 50;
            Height = 5;
            if (ModX >= 0)
                ModX += -Speed;
            else
                TakeDamage(HP);
        }

        public override void MoveRight()
        {
            Width = 50;
            Height = 5;
            if (ModX <= 1000)
                ModX += Speed;
            else
                TakeDamage(HP);
        }
    }




    public abstract class Enemy : MovableGameObject
    {
        private Timer _spawnTimer = new Timer();
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



    public class EntitiesGroup
    {
        private List<GameObject> _entities = new List<GameObject>();

        public EntitiesGroup()
        { }

        public List<GameObject> EntitiesList
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        public void AddObject(GameObject a)
        {
            EntitiesList.Add(a);
        }

        public void Display()
        {
            foreach (GameObject a in _entities)
            {
                a.DisplayItself();
            }
        }

        public void DeathCheck()
        {
            foreach (MovableGameObject a in EntitiesList.ToList())
            {
                if (a.HP <= 0)
                {
                    EntitiesList.Remove(a);
                }
            }
        }
    }



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



    public abstract class GameObject
    {
        private Color _color;
        private int _x, _y, _width = 40, _height = 40;
        private ObjectType _hostility = ObjectType.neutral;

        public GameObject(Color color, int x, int y)
        {
            Color = color;
            ModX = x;
            ModY = y;
        }

        public ObjectType Hostility
        {
            get
            {
                return _hostility;
            }
            set
            {
                _hostility = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public int ModX
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public int ModY
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }


        //cordinate is from the outliner
        public virtual void DisplayItself()
        {
            SwinGame.FillRectangle(Color.Black, ModX, ModY, Width, Height);
            SwinGame.FillRectangle(Color, ModX + 1, ModY + 1, Width - 2, Height - 2);
        }

        public bool PlayerCollision(Player p)
        {
            if (Hostility == ObjectType.hostile)
            {
                //if collide from the left
                if (ModX <= p.ModX)
                {
                    //from top or from bottom
                    if (p.ModX - ModX < Width && ((ModY <= p.ModY && p.ModY - ModY < Height) || (ModY >= p.ModY && ModY - p.ModY < p.Height)))
                        return true;
                }
                //collide from the right
                else if (ModX >= p.ModX)
                {
                    if (ModX - p.ModX < p.Width && ((ModY <= p.ModY && p.ModY - ModY < Height) || (ModY >= p.ModY && ModY - p.ModY < p.Height)))
                        return true;
                }
            }
            return false;
        }
    }




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





    interface IDetectBullets
    {
        void DetectBullets(EntitiesGroup bulletEntities);
        int HP { get; set; }
        void UpdateHPColor();
    }




    public abstract class MovableGameObject : GameObject
    {
        private int _spd = 0, _health, _baseHP;

        public MovableGameObject(Color color, int hp, int x, int y) : base(color, x, y)
        {
            HP = hp;
            _baseHP = hp;
        }

        public int BaseHP
        {
            get
            {
                return _baseHP;
            }
        }

        public int HP
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public int Speed
        {
            get
            {
                return _spd;
            }
            set
            {
                _spd = value;
            }
        }

        //movement cannot be made if the object would go pass the screen border
        public virtual void MoveUp()
        {
            if (ModY > 150 && Hostility != ObjectType.frozen)
                ModY += -Speed;
        }

        public virtual void MoveDown()
        {
            if (ModY < 610 && Hostility != ObjectType.frozen)
                ModY += Speed;
        }

        public virtual void MoveLeft()
        {
            if (ModX > 0 && Hostility != ObjectType.frozen)
                ModX += -Speed;
        }

        public virtual void MoveRight()
        {
            if (ModX < 960 && Hostility != ObjectType.frozen)
                ModX += Speed;
        }

        //take damage
        public void TakeDamage(int damage)
        {
            HP += -damage;
            if (HP < 0)
                HP = 0;
        }
    }




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
            if (HP < 10)
                SwinGame.DrawText("0" + HP.ToString(), Color.White, optimusFont, 150, 30);
            else
                SwinGame.DrawText(HP.ToString(), Color.White, optimusFont, 150, 30);
            SwinGame.DrawText("/" + BaseHP.ToString(), Color.White, optimusFont, 190, 30);
            SwinGame.DrawText("HP", Color.White, optimusFont, 250, 30);
            //kill counter
            SwinGame.DrawText("Kills: " + _killCount.ToString(), Color.White, optimusFont, 750, 80);
            //ammo
            if (Weapon.Reloading == false)
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
            if (SkillCharged)
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
            if (SkillCharged)
                SwinGame.DrawRectangle(Color.Green, ModX - 110, ModY - 110, 260, 260);
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




    public class SwiperEnemy : Enemy
    {
        private Direction _dir = Direction.down;

        public SwiperEnemy(int hp, Player p) : base(hp, 0, 0, p)
        {
            Width = 1100;
            Height = 14;
        }

        public override void MoveDown()
        {
            ModY += Speed;
        }

        public override void MoveUp()
        {
            ModY -= Speed;
        }

        public override void SpecialMove()
        {
            if (_dir == Direction.down)
                MoveDown();
            else
                MoveUp();
            if (ModY > 670)
                _dir = Direction.up;
            if (ModY < 120)
                _dir = Direction.down;
        }

        public override void DisplayItself()
        {
            if (SpawnTimer.Ticks > 3000)
            {
                SpawnTimer.Pause();
                base.DisplayItself();
                Hostility = ObjectType.hostile;
                Speed = 2;
            }
            else
            {
                SwinGame.FillRectangle(Color.Orange, ModX, ModY, Width, Height);
            }
        }
    }





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
                if (_idle)
                    Color = Color.DarkBlue;
                else
                    UpdateHPColor();
                SpawnTimer.Pause();
                base.DisplayItself();
                Hostility = ObjectType.hostile;
                Speed = 0;
            }
            else if (SpawnTimer.Ticks == 3000)
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
                            if (_skillChargeTime.Ticks < 2000 && _cordLocked == false)
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
            if (_skillSpacing.Ticks > _spacingMultiplier * 500)
            {
                _skillSpacing.Stop();
                _idle = false;
                _skillChargeTime.Start();
            }
            if (!_idle)
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
            switch (state)
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









    public class GameMain
    {
        public static void Main()
        {
            //Open the game window
            SwinGame.OpenGraphicsWindow("Blocks", 1000, 650);
            SwinGame.ShowSwinGameSplashScreen();
            //setting up sound FX and font
            var optimusFont = SwinGame.LoadFont("Optimus.otf", 30);
            var titleFont = SwinGame.LoadFont("Optimus.otf", 100);
            SwinGame.OpenAudio();
            var pewFX = SwinGame.LoadSoundEffect("pew.ogg");
            var oofFX = SwinGame.LoadSoundEffect("oof.ogg");
            var fortnitededFX = SwinGame.LoadSoundEffect("fortniteded.ogg");

            //setting up bitmap
            Bitmap freeze = SwinGame.LoadBitmap("freeze.jpg");
            Bitmap speedBoost = SwinGame.LoadBitmap("speedboost.jpg");

            //checkpoint for restarting game
            bool gamestart = false;

            //pausing
            bool pause = false;

            //input difficulty
            int difficulty = 1;
            int waveCount = 1;

            //initiating objects
            Player p = new Player();
            EnemyHorde horde = new EnemyHorde(difficulty, p);
            Timer ReloadTimer = new Timer();
            Timer StageTransitionTimer = new Timer();
            EntitiesGroup enemyEntities = new EntitiesGroup();
            EntitiesGroup playerEntity = new EntitiesGroup();
            EntitiesGroup bulletEntities = new EntitiesGroup();
            EntitiesGroup powerUpEntities = new EntitiesGroup();
            PowerUpSpawner powerUpSpawner = new PowerUpSpawner(p, new Bitmap[] { freeze, speedBoost }, enemyEntities);
            playerEntity.AddObject(p);



            //Run the game loop
            while (false == SwinGame.WindowCloseRequested())
            {
                //getting user input
                SwinGame.ProcessEvents();


                //if the player has died and been noticed "Game Over" or this is the first startup
                while (gamestart == false && false == SwinGame.WindowCloseRequested())
                {
                    //getting difficulty and reset waves count
                    waveCount = 1;
                    SwinGame.ProcessEvents();
                    SwinGame.DrawText("Welcome to a dumb shooter game", Color.Black, optimusFont, 250, 100);
                    //title screen
                    SwinGame.DrawText("B", Color.Black, titleFont, 323, 153);
                    SwinGame.DrawText("L", Color.Black, titleFont, 388, 163);
                    SwinGame.DrawText("O", Color.Black, titleFont, 453, 153);
                    SwinGame.DrawText("C", Color.Black, titleFont, 518, 163);
                    SwinGame.DrawText("K", Color.Black, titleFont, 583, 153);
                    SwinGame.DrawText("S", Color.Black, titleFont, 648, 163);
                    SwinGame.DrawText("B", Color.Purple, titleFont, 320, 150);
                    SwinGame.DrawText("L", Color.Red, titleFont, 385, 160);
                    SwinGame.DrawText("O", Color.Yellow, titleFont, 450, 150);
                    SwinGame.DrawText("C", Color.Green, titleFont, 515, 160);
                    SwinGame.DrawText("K", Color.Blue, titleFont, 580, 150);
                    SwinGame.DrawText("S", Color.Pink, titleFont, 645, 160);
                    SwinGame.DrawText("Press H to open up the tutorial", Color.Green, optimusFont, 80, 400);
                    SwinGame.DrawText("This game has 3 difficulties - Easy, Normal or Hard.", Color.Purple, optimusFont, 80, 450);
                    SwinGame.DrawText("Press 1 (Easy), 2 (Normal) or 3 (Hard) to start!", Color.Purple, optimusFont, 80, 500);
                    SwinGame.RefreshScreen(60);

                    //pressing H will open the help screen
                    if (SwinGame.KeyTyped(KeyCode.HKey))
                    {
                        while (false == SwinGame.WindowCloseRequested())
                        {
                            SwinGame.ProcessEvents();
                            //clearing screen to white
                            SwinGame.ClearScreen(Color.White);
                            SwinGame.DrawText("In this game, you are a black block.", Color.Black, optimusFont, 50, 50);
                            SwinGame.DrawText("Basically you just shoot anything that's not you.", Color.Black, optimusFont, 50, 100);
                            SwinGame.DrawText("If you touch other blocks, you lose HP equal to theirs", Color.Black, optimusFont, 50, 150);
                            SwinGame.DrawText("ESC - Pause", Color.Black, optimusFont, 50, 200);
                            SwinGame.DrawText("W A S D - Move", Color.Black, optimusFont, 50, 250);
                            SwinGame.DrawText("Arrow keys - shoot at a direction", Color.Black, optimusFont, 50, 300);
                            SwinGame.DrawText("R - reload", Color.Black, optimusFont, 50, 350);
                            SwinGame.DrawText("W/A/S/D + LEFT SHIFT - teleport", Color.Black, optimusFont, 50, 400);
                            SwinGame.DrawText("The range of teleportation is the green square around you", Color.Black, optimusFont, 50, 450);
                            SwinGame.DrawText("Teleportation cooldown: 1.5s | Reload time: 1s", Color.Black, optimusFont, 50, 500);
                            SwinGame.DrawText("Press H again or ESC to return to the main screen", Color.Black, optimusFont, 50, 550);
                            //drawing things out
                            SwinGame.RefreshScreen(60);
                            if (SwinGame.KeyTyped(KeyCode.HKey) || SwinGame.KeyTyped(KeyCode.EscapeKey))
                            {
                                SwinGame.ClearScreen(Color.White);
                                break;
                            }
                        }
                    }
                    if (SwinGame.KeyTyped(KeyCode.Num1Key))
                    {
                        difficulty = 1;
                        gamestart = true;
                    }
                    if (SwinGame.KeyTyped(KeyCode.Num2Key))
                    {
                        difficulty = 2;
                        gamestart = true;
                    }
                    if (SwinGame.KeyTyped(KeyCode.Num3Key))
                    {
                        difficulty = 3;
                        gamestart = true;
                    }
                    //renewing objects
                    p = new Player();
                    horde = new EnemyHorde(difficulty, p);
                    ReloadTimer = new Timer();
                    StageTransitionTimer = new Timer();
                    enemyEntities = new EntitiesGroup();
                    playerEntity = new EntitiesGroup();
                    bulletEntities = new EntitiesGroup();
                    powerUpEntities = new EntitiesGroup();
                    powerUpSpawner = new PowerUpSpawner(p, new Bitmap[] { freeze, speedBoost }, enemyEntities);
                    playerEntity.AddObject(p);
                    foreach (Enemy e in horde.EnemyList)
                        enemyEntities.AddObject(e);
                }



                //------------------------------------------------------------------------------------
                //PLAYER MECHANICS
                //------------------------------------------------------------------------------------

                //player control
                if (SwinGame.KeyDown(KeyCode.WKey))
                {
                    if (SwinGame.KeyTyped(KeyCode.ShiftKey))
                        p.Teleport(Direction.up);
                    else
                        p.MoveUp();
                }
                if (SwinGame.KeyDown(KeyCode.AKey))
                {
                    if (SwinGame.KeyTyped(KeyCode.ShiftKey))
                        p.Teleport(Direction.left);
                    else
                        p.MoveLeft();
                }
                if (SwinGame.KeyDown(KeyCode.SKey))
                {
                    if (SwinGame.KeyTyped(KeyCode.ShiftKey))
                        p.Teleport(Direction.down);
                    else
                        p.MoveDown();
                }
                if (SwinGame.KeyDown(KeyCode.DKey))
                {
                    if (SwinGame.KeyTyped(KeyCode.ShiftKey))
                        p.Teleport(Direction.right);
                    else
                        p.MoveRight();
                }


                //player regen
                p.Regenerate();

                //player shooting
                if (SwinGame.KeyTyped(KeyCode.UpKey) && p.Weapon.Round != 0)
                {
                    bulletEntities.AddObject(p.Weapon.Shoot(Direction.up, p.ModX, p.ModY, pewFX));
                }
                if (SwinGame.KeyTyped(KeyCode.LeftKey) && p.Weapon.Round != 0)
                {
                    bulletEntities.AddObject(p.Weapon.Shoot(Direction.left, p.ModX, p.ModY, pewFX));
                }
                if (SwinGame.KeyTyped(KeyCode.DownKey) && p.Weapon.Round != 0)
                {
                    bulletEntities.AddObject(p.Weapon.Shoot(Direction.down, p.ModX, p.ModY, pewFX));
                }
                if (SwinGame.KeyTyped(KeyCode.RightKey) && p.Weapon.Round != 0)
                {
                    bulletEntities.AddObject(p.Weapon.Shoot(Direction.right, p.ModX, p.ModY, pewFX));
                }

                //player reloading
                if (SwinGame.KeyTyped(KeyCode.RKey) && p.Weapon.Reloading == false)
                {
                    ReloadTimer.Start();
                    p.Weapon.Reloading = true;
                }

                //reloading delay handler
                if (ReloadTimer.Ticks > 1000)
                {
                    ReloadTimer.Stop();
                    p.Weapon.Reload();
                }





                //------------------------------------------------------------------------------------
                //BULLET MECHANICS
                //------------------------------------------------------------------------------------

                //bullet shot flying physics
                foreach (Bullet b in bulletEntities.EntitiesList)
                {
                    b.Fly();
                }




                //------------------------------------------------------------------------------------
                //POWER UPS MECHANICS
                //------------------------------------------------------------------------------------

                //power ups spawner
                powerUpSpawner.SpawnPowerUps(powerUpEntities);




                //------------------------------------------------------------------------------------
                //ENEMY MECHANICS
                //------------------------------------------------------------------------------------

                //enemy collision handler - collide with player
                foreach (Enemy e in horde.EnemyList)
                {
                    if (e.PlayerCollision(p))
                    {
                        p.TakeDamage(e.HP);
                        e.TakeDamage(e.HP);
                    }
                }

                foreach (Enemy e in enemyEntities.EntitiesList)
                {
                    if (e as IDetectBullets != null)
                        //bullet hit handler
                        (e as IDetectBullets).DetectBullets(bulletEntities);

                    //enemy death sound effect and kill counting
                    if (e.HP <= 0)
                    {
                        SwinGame.PlaySoundEffect(oofFX);
                        p.Kill++;
                    }
                }

                //Body remover (0 HP entities or timed out power ups will be removed)
                enemyEntities.DeathCheck();
                bulletEntities.DeathCheck();
                foreach (Enemy e in horde.EnemyList.ToList())
                {
                    if (e.HP <= 0)
                        horde.EnemyList.Remove(e);
                }
                foreach (PowerUp pwr in powerUpEntities.EntitiesList.ToList())
                {
                    if (pwr.EffectTimedOut())
                    {
                        pwr.RevertEffect();
                        powerUpEntities.EntitiesList.Remove(pwr);
                    }
                }

                //new horde when no enemy is left
                if (enemyEntities.EntitiesList.Count == 0 || (difficulty == 3 && horde.EnemyList.Count == 1))
                {
                    waveCount++;
                    enemyEntities.EntitiesList.Clear();
                    horde.NewWave(p);
                    foreach (Enemy e in horde.EnemyList)
                        enemyEntities.AddObject(e);
                }

                //Clear the screen
                SwinGame.ClearScreen(Color.White);

                //Pausing the game
                if (SwinGame.KeyTyped(KeyCode.EscapeKey))
                {
                    pause = true;
                    while (false == SwinGame.WindowCloseRequested() && pause == true)
                    {
                        SwinGame.ProcessEvents();
                        SwinGame.DrawText("PAUSED", Color.Black, optimusFont, 439, 279);
                        SwinGame.DrawText("PAUSED", Color.Blue, optimusFont, 440, 280);
                        SwinGame.DrawText("Press SPACE to continue", Color.Black, optimusFont, 329, 319);
                        SwinGame.DrawText("Press SPACE to continue", Color.Purple, optimusFont, 330, 320);
                        SwinGame.RefreshScreen(60);
                        SwinGame.ClearScreen(Color.White);
                        if (SwinGame.KeyTyped(KeyCode.SpaceKey))
                        {
                            pause = false;
                        }
                    }
                }

                //Drawing entities
                foreach (Bullet bullet in bulletEntities.EntitiesList)
                {
                    if (bullet.FlyingDirection != Direction.none)
                        bullet.DisplayItself();
                }
                powerUpEntities.Display();
                enemyEntities.Display();
                playerEntity.Display();

                //bot control
                foreach (Enemy e in enemyEntities.EntitiesList)
                {
                    e.SpecialMove();
                    //dummyPointer = e as NormalEnemy;
                    //if (dummyPointer != null)
                    //    dummyPointer.Repositioning(horde);
                }

                //Drawing HUD
                SwinGame.FillRectangle(Color.Black, 0, 0, 1000, 150);
                SwinGame.FillRectangle(Color.Purple, 0, 145, 1000, 5);

                //telling player to draw out its details
                foreach (Player Ps in playerEntity.EntitiesList)
                    Ps.DisplayPlayerDetails(optimusFont);

                //draw framerate
                SwinGame.DrawFramerate(0, 0);

                //display current wave
                SwinGame.DrawText("Wave: " + waveCount.ToString(), Color.White, optimusFont, 750, 30);

                //wave starting countdown
                if (horde.EnemyList[0].SpawnTimer.Ticks > 0 && horde.EnemyList[0].SpawnTimer.Ticks < 1000)
                {
                    SwinGame.DrawText("3", Color.DarkGray, optimusFont, 492, 332);
                    SwinGame.DrawText("3", Color.Yellow, optimusFont, 490, 330);
                }
                if (horde.EnemyList[0].SpawnTimer.Ticks >= 1000 && horde.EnemyList[0].SpawnTimer.Ticks < 2000)
                {
                    SwinGame.DrawText("2", Color.DarkGray, optimusFont, 492, 332);
                    SwinGame.DrawText("2", Color.Orange, optimusFont, 490, 330);
                }
                if (horde.EnemyList[0].SpawnTimer.Ticks >= 2000 && horde.EnemyList[0].SpawnTimer.Ticks < 3000)
                {
                    SwinGame.DrawText("1", Color.DarkGray, optimusFont, 492, 332);
                    SwinGame.DrawText("1", Color.Red, optimusFont, 490, 330);
                }



                //game over screen
                if (p.HP <= 0)
                {
                    SwinGame.PlaySoundEffect(fortnitededFX);
                    SwinGame.ClearScreen(Color.White);
                    SwinGame.DrawText("GAME OVER!", Color.Black, optimusFont, 399, 279);
                    SwinGame.DrawText("GAME OVER!", Color.Red, optimusFont, 400, 280);
                    SwinGame.DrawText("You survived " + waveCount.ToString() + " waves", Color.Black, optimusFont, 349, 349);
                    SwinGame.DrawText("You survived " + waveCount.ToString() + " waves", Color.Green, optimusFont, 350, 350);
                    SwinGame.DrawText("You destroyed " + p.Kill.ToString() + " blocks", Color.Black, optimusFont, 349, 399);
                    SwinGame.DrawText("You destroyed " + p.Kill.ToString() + " blocks", Color.Green, optimusFont, 350, 400);
                    SwinGame.ReleaseResourceBundle("soundFX.txt");
                    SwinGame.RefreshScreen(60);
                    gamestart = false;
                    SwinGame.Delay(3000);
                    SwinGame.ClearScreen(Color.White);
                    p.HP = 1;
                }

                //Draw onto the screen
                SwinGame.RefreshScreen(60);
            }
            SwinGame.FreeBitmap(freeze);
            SwinGame.FreeBitmap(speedBoost);
        }
    }
}
