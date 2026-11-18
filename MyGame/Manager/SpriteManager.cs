
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
    private Texture2D playerPistolTexture;
    private Texture2D playerShotgunTexture;
    private Texture2D zombieTexture;
    private WeaponManager weaponManager;
    private WaveManager waveManager;
    private readonly Random random = new Random();
    
    // Check
    //Frame size and scale of the zombie sprite, used whenever a zombie is created.
    private static readonly Point ZombieFrameSize = new Point(322, 276);
    private const float ZombieScale = 0.35f;
    private const float ZombieSpeed = 2.5f;

    //Every enemy that is currently alive.
    private List<Enemy> enemies = new List<Enemy>();
    private List<Sprite> spriteList = new List<Sprite>();
    public event Action<int> ZombieKilled;
    public event Action WaveCompleted;
    public int PlayerHealth => player.Health;
    public int PlayerMaxHealth => player.MaxHealth;
    public float PlayerStamina => player.Stamina;
    public float PlayerMaxStamina => player.MaxStamina;
    public int CurrentWave => waveManager.CurrentWave;
    public string CurrentWaveRomanNumeral => waveManager.GetRomanNumeral();

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

        playerPistolTexture = Game.Content.Load<Texture2D>("PlayerPistol");
        playerShotgunTexture = Game.Content.Load<Texture2D>("PlayerShotgun");
        playerSprite = new PlayerSprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale);
            playerPistolTexture, Vector2.Zero, new Point(playerPistolTexture.Width, playerPistolTexture.Height),
            10, new Point(0,0), new Point(1,1), new Vector2(6,6), 0.4f);
        
        player = new Player(playerSprite, 100, 100);

        zombieTexture = Game.Content.Load<Texture2D>("ZombieSprite");

        waveManager = new WaveManager();
        waveManager.Reset();
        SpawnWave();

        spriteList.Add(playerSprite);
        // spriteList.Add(enemySprite);

        //1x1 white image that the bullets stretch into little rectangles.
        Texture2D bulletPixel = new Texture2D(Game.GraphicsDevice, 1, 1);
        bulletPixel.SetData(new[] { Color.White });

        weaponManager = new WeaponManager(bulletPixel);
        
        base.LoadContent();
    }

    // =================
    // TEMP
    // =================
    //Raised when an enemy dies; the value is the money it pays out.
    public event Action<int> EnemyKilled;
    //Passes the shop state (shotgun owned, upgrade levels) on to the weapons.
    public void ApplyShopState(bool shotgunPurchased, int pistolUpgradeLevel, int shotgunUpgradeLevel)
    {
        weaponManager?.ApplyShopState(shotgunPurchased, pistolUpgradeLevel, shotgunUpgradeLevel);
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        base.OnEnabledChanged(sender, args);

        //When gameplay becomes active again, the mouse click that started or
        //resumed the game must not also fire a shot.
        if (Enabled)
            weaponManager?.RequireTriggerRelease();
    }

    public void ResetForNewGame()
    {
        playerSprite.SetAppearance(playerPistolTexture);
        Rectangle bounds = Game.GraphicsDevice.Viewport.Bounds;
        Vector2 spawnPosition = new(
            (bounds.Width - playerSprite.FrameSize.X * playerSprite.Scale) / 2f,
            (bounds.Height - playerSprite.FrameSize.Y * playerSprite.Scale) / 2f);
        player.Respawn(spawnPosition);
        enemies.Clear();
        spriteList.Clear();
        spriteList.Add(playerSprite);
        waveManager.Reset();
        weaponManager.Reset();
        SpawnWave();
    }

    public void StartNextWave(bool shotgunPurchased)
    {
        player.RestoreHealth();
        if (shotgunPurchased)
        {
            playerSprite.SetAppearance(playerShotgunTexture);
            weaponManager.EquipShotgun();
        }
    }

    public void SetWeaponUpgradeLevels(bool shotgun, int fireRateLevel, int damageLevel)
    {
        weaponManager.SetUpgradeLevels(shotgun, fireRateLevel, damageLevel);
    }

    private void SpawnZombie(Vector2 position)
    {
        EnemySprite zombieSprite = new EnemySprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale, playerSprite);
            zombieTexture, position, ZombieFrameSize, 10, new Point(0,0), new Point(1,1),
            new Vector2(ZombieSpeed, ZombieSpeed), ZombieScale, player.Sprite);

        enemies.Add(new Zombie(zombieSprite, position, ZombieSpeed, 100));
        spriteList.Add(zombieSprite);
    }

    private void SpawnWave()
    {
        Rectangle screenBounds = Game.GraphicsDevice.Viewport.Bounds;

        for (int i = 0; i < waveManager.EnemiesInCurrentWave; i++)
        {
            SpawnZombie(GetRandomSpawnPosition(screenBounds));
        }
    }

    private Vector2 GetRandomSpawnPosition(Rectangle screenBounds)
    {
        int width = (int)(ZombieFrameSize.X * ZombieScale);
        int height = (int)(ZombieFrameSize.Y * ZombieScale);
        int minX = screenBounds.Left;
        int maxX = Math.Max(minX, screenBounds.Right - width);
        int minY = screenBounds.Top;
        int maxY = Math.Max(minY, screenBounds.Bottom - height);
        Vector2 candidate = Vector2.Zero;

        for (int attempt = 0; attempt < 100; attempt++)
        {
            int side = random.Next(4);
            candidate = side switch
            {
                0 => new Vector2(screenBounds.Left - width - 2, random.Next(minY, maxY + 1)),
                1 => new Vector2(screenBounds.Right + 2, random.Next(minY, maxY + 1)),
                2 => new Vector2(random.Next(minX, maxX + 1), screenBounds.Top - height - 2),
                _ => new Vector2(random.Next(minX, maxX + 1), screenBounds.Bottom + 2)
            };

            if (IsSpawnPositionFree(candidate))
                return candidate;
        }

        float rightOfAllSprites = screenBounds.Right + 2;

        foreach (Enemy enemy in enemies)
        {
            rightOfAllSprites = Math.Max(
                rightOfAllSprites,
                enemy.Sprite.Position.X + enemy.Sprite.FrameSize.X * enemy.Sprite.Scale);
        }

        return new Vector2(rightOfAllSprites + 2, screenBounds.Top);
    }

    private bool IsSpawnPositionFree(Vector2 position)
    {
        int width = (int)(ZombieFrameSize.X * ZombieScale);
        int height = (int)(ZombieFrameSize.Y * ZombieScale);
        Rectangle candidate = new Rectangle(
            (int)position.X - 1,
            (int)position.Y - 1,
            width + 2,
            height + 2);

        if (candidate.Intersects(GetSeparatedBounds(playerSprite, playerSprite.Position)))
            return false;

        foreach (Enemy enemy in enemies)
        {
            if (candidate.Intersects(GetSeparatedBounds(enemy.Sprite, enemy.Sprite.Position)))
                return false;
        }

        return true;
    }

    //Removes enemies that have no health left.
    private void RemoveDeadEnemies(Rectangle screenBounds)
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].IsDead)
            {
                ZombieKilled?.Invoke(500);
                spriteList.Remove(enemies[i].Sprite);
                enemies.RemoveAt(i);
            }
        }

        if (enemies.Count == 0)
        {
            waveManager.AdvanceWave();
            SpawnWave();
            WaveCompleted?.Invoke();
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
        Rectangle nextBounds = GetSpriteBounds(movingSprite, newPosition);

        //Keep the player inside the screen.
        if (constrainToScreen &&
            (nextBounds.Left < screenBounds.Left ||
             nextBounds.Top < screenBounds.Top ||
             nextBounds.Right > screenBounds.Right ||
             nextBounds.Bottom > screenBounds.Bottom))
        {
            return false;
        }

        if (movingSprite is EnemySprite)
        {
            Rectangle separatedBounds = GetSeparatedBounds(movingSprite, newPosition);

            foreach (Enemy enemy in enemies)
            {
                if (ReferenceEquals(enemy.Sprite, movingSprite))
                    continue;

                if (separatedBounds.Intersects(
                    GetSeparatedBounds(enemy.Sprite, enemy.Sprite.Position)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static Rectangle GetSpriteBounds(Sprite sprite, Vector2 position)
    {
        return new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(sprite.FrameSize.X * sprite.Scale),
            (int)(sprite.FrameSize.Y * sprite.Scale));
    }

    private static Rectangle GetSeparatedBounds(Sprite sprite, Vector2 position)
    {
        Rectangle bounds = GetSpriteBounds(sprite, position);
        return new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
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