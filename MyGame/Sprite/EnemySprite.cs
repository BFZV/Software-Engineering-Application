using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MyGame.Sprite;
public class EnemySprite: Sprite
{
    private PlayerSprite playerSprite;
    public EnemySprite(Texture2D textureImage, Vector2 position, Point frameSize,
     int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, float scale, PlayerSprite playerSprite):
     base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
    {
        this.scale = scale;
        this.playerSprite = playerSprite;
    }

    public override Vector2 direction
    {
        get
        {
            if (playerSprite == null)
                return Vector2.Zero;

            Vector2 enemyCenter = position +
                new Vector2(frameSize.X, frameSize.Y) * scale / 2f;

            Vector2 playerCenter = playerSprite.Position +
                new Vector2(playerSprite.FrameSize.X, playerSprite.FrameSize.Y)
                * playerSprite.Scale / 2f;

            Vector2 direction = playerCenter - enemyCenter;

            if (direction != Vector2.Zero)
                direction.Normalize();

            return direction * speed;
        }
    }
    

    private void UpdateRotation()
    {
        if (playerSprite == null)
        return;

        Vector2 enemyCenter = position +
            new Vector2(frameSize.X, frameSize.Y) * scale / 2f;

        Vector2 playerCenter = playerSprite.Position +
            new Vector2(
                playerSprite.FrameSize.X,
                playerSprite.FrameSize.Y
            ) * playerSprite.Scale / 2f;

        Vector2 difference = playerCenter - enemyCenter;

        // Don't change rotation if the centers are almost identical.
        if (difference.LengthSquared() < 1.0f)
            return;

        rotation = (float)Math.Atan2(
            difference.Y,
            difference.X
        );

        // // Find the center of the enemy on screen.
        // Vector2 enemyCenter = position +
        // new Vector2(frameSize.X, frameSize.Y) * scale / 2f;

        // Vector2 playerCenter = playerSprite.Position + 
        // new Vector2(playerSprite.FrameSize.X, playerSprite.FrameSize.Y) * playerSprite.Scale / 2f;

        // // Find the direction from the enemy to the player.
        // float deltaX = playerCenter.X - enemyCenter.X;
        // float deltaY = playerCenter.Y - enemyCenter.Y;

        // rotation = (float)Math.Atan2(deltaY, deltaX);
    }

    public override void Update(GameTime gameTime, Rectangle clientBounds)
    {

    UpdateRotation();

    base.Update(gameTime, clientBounds);

    }
}