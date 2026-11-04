
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyGame.Sprite;
public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
{
    private SpriteBatch spriteBatch;
    private PlayerSprite playerSprite;
    private Player player;
    private EnemySprite enemySprite;

    //Debugging tool
    private SpriteFont font;

    private List<EnemySprite> enemyList =
    new List<EnemySprite>();
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

        enemySprite = new EnemySprite(
            //textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, scale, playerSprite);
            Game.Content.Load<Texture2D>("ZombieSprite"),new Vector2(300, 300), 
            new Point(322,276), 10, new Point(0,0), new Point(1,1), new Vector2(5,5),
            0.5f, player.Sprite);

        spriteList.Add(playerSprite);
        spriteList.Add(enemySprite);
        
        base.LoadContent();
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

        //Move the enemy toward the player.
        MoveSprite(
            enemySprite,
            enemySprite.direction,
            false,
            bounds
        );

        //Check for collisions between the player and enemy.
        if (!player.IsDead && 
        player.Sprite.collisionRect.Intersects(enemySprite.collisionRect))
        {
            player.TakeDamage(10);    
        }

        //Update animations.
        if(!player.IsDead)
        {
            player.Sprite.Update(gameTime, bounds);
        }

        enemySprite.Update(gameTime, bounds);

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
        enemySprite.Draw(gameTime, spriteBatch);
        spriteBatch.End();
        base.Draw(gameTime);
    }




}