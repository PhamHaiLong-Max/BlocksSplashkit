using System;
using System.Linq;
using SwinGameSDK;

namespace MyGame
{
    public enum Direction { up, down, left, right, none}

    public enum ObjectType { neutral, hostile, frozen}

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
            PowerUpSpawner powerUpSpawner = new PowerUpSpawner(p, new Bitmap[] { freeze, speedBoost}, enemyEntities);
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
                foreach(Enemy e in horde.EnemyList.ToList())
                {
                    if (e.HP <= 0)
                        horde.EnemyList.Remove(e);
                }
                foreach(PowerUp pwr in powerUpEntities.EntitiesList.ToList())
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
                    SwinGame.DrawText("You destroyed " + p.Kill.ToString() +" blocks", Color.Black, optimusFont, 349, 399);
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