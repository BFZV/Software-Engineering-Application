
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MyGame;
public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
{
    private SpriteBatch spriteBatch;
    private PlayerSprite playerSprite;
    private Player player;
    private Texture2D zombieTexture;
    private WeaponManager weaponManager;
    
    // Check
    //Frame size and scale of the zombie sprite, used whenever a zombie is created.
    private static readonly Point ZombieFrameSize = new Point(322, 276);
    private const float ZombieScale = 0.5f;

    //Debugging tool
    private SpriteFont font;

    //Every enemy that is currently alive.
    private List<Enemy> enemies = new List<Enemy>();
    private List<Sprite> spriteList = new List<Sprite>();
    public SpriteManager(Game game) : base(game)
    {
    }
    
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(Game.GraphicsDevice);

        //Debugging tool
        font = Game.Content.Load<SpriteFont>("GameFont");

        playerSprite = new PlayerSprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale);
            Game.Content.Load<Texture2D>("PlayerSprite"),Vector2.Zero, new Point(438,269),
            10, new Point(0,0), new Point(1,1), new Vector2(6,6), 0.5f);
        
        player = new Player(playerSprite, 100, 100);

        /*
        enemySprite = new EnemySprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale, playerSprite);
            Game.Content.Load<Texture2D>("ZombieSprite"),new Vector2(300, 300), 
            new Point(322,276), 10, new Point(0,0), new Point(1,1), new Vector2(5,5),
            0.5f, player.Sprite);
        */

        zombieTexture = Game.Content.Load<Texture2D>("ZombieSprite");

        SpawnZombie(new Vector2(300, 300));

        spriteList.Add(playerSprite);
        // spriteList.Add(enemySprite);

        //1x1 white image that the bullets stretch into little rectangles.
        Texture2D bulletPixel = new Texture2D(Game.GraphicsDevice, 1, 1);
        bulletPixel.SetData(new[] { Color.White });

        weaponManager = new WeaponManager(bulletPixel);
        
        base.LoadContent();
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        base.OnEnabledChanged(sender, args);

        //When gameplay becomes active again, the mouse click that started or
        //resumed the game must not also fire a shot.
        if (Enabled)
            weaponManager?.RequireTriggerRelease();
    }

    private void SpawnZombie(Vector2 position)
    {
        EnemySprite zombieSprite = new EnemySprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale, playerSprite);
            zombieTexture, position, ZombieFrameSize, 10, new Point(0,0), new Point(1,1),
            new Vector2(5,5), ZombieScale, player.Sprite);

        enemies.Add(new Zombie(zombieSprite, position, 5f, 100));
        spriteList.Add(zombieSprite);
    }

    //Removes enemies that have no health left.
    private void RemoveDeadEnemies(Rectangle screenBounds)
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].IsDead)
            {
                spriteList.Remove(enemies[i].Sprite);
                enemies.RemoveAt(i);
            }
        }

        //TEMPORARY: there is no wave spawner yet, so a new zombie walks in whenever
        //the last one dies. This keeps the weapons testable. Replace with wave logic later.
        if (enemies.Count == 0)
        {
            SpawnZombie(GetCornerFarthestFromPlayer(screenBounds));
        }
    }

    //Picks the screen corner farthest from the player so a new zombie never appears on top of them.
    private Vector2 GetCornerFarthestFromPlayer(Rectangle screenBounds)
    {
        float zombieWidth = ZombieFrameSize.X * ZombieScale;
        float zombieHeight = ZombieFrameSize.Y * ZombieScale;

        Vector2[] corners =
        {
            new Vector2(screenBounds.Left, screenBounds.Top),
            new Vector2(screenBounds.Right - zombieWidth, screenBounds.Top),
            new Vector2(screenBounds.Left, screenBounds.Bottom - zombieHeight),
            new Vector2(screenBounds.Right - zombieWidth, screenBounds.Bottom - zombieHeight)
        };

        Vector2 playerCenter = player.Sprite.Center;
        Vector2 farthest = corners[0];

        foreach (Vector2 corner in corners)
        {
            if (Vector2.DistanceSquared(corner, playerCenter) >
                Vector2.DistanceSquared(farthest, playerCenter))
            {
                farthest = corner;
            }
        }

        return farthest;
    }

    private bool CanMoveTo(Sprite movingSprite,Vector2 newPosition,
    bool constrainToScreen, Rectangle screenBounds)
    {
        Rectangle nextBounds = new Rectangle(
            (int)newPosition.X,
            (int)newPosition.Y,
            (int)(movingSprite.FrameSize.X * movingSprite.Scale),
            (int)(movingSprite.FrameSize.Y * movingSprite.Scale)
        );

        //Keep the player inside the screen.
        if (constrainToScreen &&
            (nextBounds.Left < screenBounds.Left ||
             nextBounds.Top < screenBounds.Top ||
             nextBounds.Right > screenBounds.Right ||
             nextBounds.Bottom > screenBounds.Bottom))
        {
            return false;
        }

        //Check against every other sprite.
        // foreach (Sprite other in spriteList)
        // {
        //     if (ReferenceEquals(other, movingSprite))
        //         continue;

        //     if (nextBounds.Intersects(other.collisionRect))
        //         return false;
        // }

        return true;
    }

    private void MoveSprite(
        Sprite sprite,
        Vector2 movement,
        bool constrainToScreen,
        Rectangle screenBounds)
    {
        Vector2 nextPosition = sprite.Position;

        //Try horizontal movement first.
        Vector2 horizontal = new Vector2(movement.X, 0);

        if (CanMoveTo(
            sprite, nextPosition + horizontal,
            constrainToScreen, screenBounds))
        {
            nextPosition.X += horizontal.X;
        }

        //Then try vertical movement.
        Vector2 vertical = new Vector2(0, movement.Y);

        if (CanMoveTo(
            sprite, nextPosition + vertical,
            constrainToScreen, screenBounds))
        {
            nextPosition.Y += vertical.Y;
        }

        sprite.SetPosition(nextPosition);    
    }

    public override void Update(GameTime gameTime)
    {
        if (!Enabled)
            return;
        
        Rectangle bounds = Game.GraphicsDevice.Viewport.Bounds;

        //Update player statistics and timers.
        player.Update(gameTime);

        //Only move the player if they are alive.
        if (!player.IsDead)
        {
            //Move the player first, keeping it inside the screen.
            MoveSprite(
                playerSprite,
                playerSprite.direction,
                true,
                bounds
            );
        }

         //Move every enemy toward the player and check whether it touches the player.
        foreach (Enemy enemy in enemies)
        {
            MoveSprite(
                enemy.Sprite,
                enemy.Sprite.direction,
                false,
                bounds
            );

            if (!player.IsDead &&
                player.Sprite.collisionRect.Intersects(enemy.Sprite.collisionRect))
            {
                player.TakeDamage(10);
            }
        }

        //Update animations.
        if(!player.IsDead)
        {
            player.Sprite.Update(gameTime, bounds);
        }

        foreach (Enemy enemy in enemies)
        {
            enemy.Sprite.Update(gameTime, bounds);
        }

        //Fire the equipped weapon and move the bullets. This runs after the player
        //sprite update so the gun aims with the newest rotation.
        weaponManager.Update(gameTime, player.Sprite, !player.IsDead, bounds, enemies);

        //Enemies that ran out of health are removed from the game.
        RemoveDeadEnemies(bounds);

        base.Update(gameTime);
    }
    
    public override void Draw(GameTime gameTime)
    {
        if (!Visible)
        return;
        
        spriteBatch.Begin();

        // Display health.
        spriteBatch.DrawString(
            font,
            "Health: " + player.Health,
            new Vector2(10, 10),
            Color.Black
        );

        // Display stamina.
        spriteBatch.DrawString(
            font,
            "Stamina: " + player.Stamina.ToString("F1"),
            new Vector2(10, 35),
            Color.Black
        );

        // Display sprint status.
        spriteBatch.DrawString(
            font,
            "Sprinting: " + player.Sprite.IsSprinting,
            new Vector2(10, 60),
            Color.Black
        );


        player.Sprite.Draw(gameTime, spriteBatch);
        foreach (Enemy enemy in enemies)
        {
            enemy.Sprite.Draw(gameTime, spriteBatch);
        }
        
        weaponManager.Draw(spriteBatch);

        spriteBatch.End();
        base.Draw(gameTime);
    }




}