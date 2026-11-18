using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MyGame;

public class PlayerSprite: Sprite
{
    public PlayerSprite(Texture2D textureImage, Vector2 position, Point frameSize,
     int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, float scale):
     base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
    {
        this.scale = scale;
    }

    public PlayerSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset,
     Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame):
     base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
    {
    }

    public bool IsSprinting { get; set; }

    public void SetAppearance(Texture2D textureImage)
    {
        _textureImage = textureImage;
        frameSize = new Point(textureImage.Width, textureImage.Height);
        currentFrame = Point.Zero;
        sheetSize = new Point(1, 1);
    }

    public override Vector2 direction
    {
        get
        {
            // Vector2 inputDirection = Vector2.Zero;
            // if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
            //     if (Keyboard.GetState().IsKeyDown(Keys.W))
            //         inputDirection.Y -= 1.5f;
            //     if (Keyboard.GetState().IsKeyDown(Keys.S))
            //         inputDirection.Y += 1.5f;
            //     if (Keyboard.GetState().IsKeyDown(Keys.A))
            //         inputDirection.X -= 1.5f;
            //     if (Keyboard.GetState().IsKeyDown(Keys.D))
            //         inputDirection.X += 1.5f; 
            // } else {
            //     if (Keyboard.GetState().IsKeyDown(Keys.W))
            //         inputDirection.Y -= 1;
            //     if (Keyboard.GetState().IsKeyDown(Keys.S))
            //         inputDirection.Y += 1;
            //     if (Keyboard.GetState().IsKeyDown(Keys.A))
            //         inputDirection.X -= 1;
            //     if (Keyboard.GetState().IsKeyDown(Keys.D))
            //         inputDirection.X += 1;
            // }

            // if (inputDirection != Vector2.Zero)
            // {
            //     inputDirection.Normalize();
            // }

            // return inputDirection * speed;

            Vector2 inputDirection = Vector2.Zero;
            KeyboardState keyboard = Keyboard.GetState();

            float multiplier = IsSprinting ? 1.5f : 1.0f;

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                inputDirection.Y -= 1;

            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                inputDirection.Y += 1;

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                inputDirection.X -= 1;

            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                inputDirection.X += 1;

            if (inputDirection != Vector2.Zero)
                inputDirection.Normalize();

            return inputDirection * speed * multiplier;
        }
    }

    private void UpdateRotation()
    {
        MouseState mouse = Mouse.GetState();

        // Find the center of the player on screen.
        Vector2 playerCenter = position +
        new Vector2(frameSize.X, frameSize.Y) * scale / 2f;

        // Find the direction from the player to the mouse.
        float deltaX = mouse.X - playerCenter.X;
        float deltaY = mouse.Y - playerCenter.Y;

        rotation = (float)Math.Atan2(deltaY, deltaX);
    }
    
    public override void Update(GameTime gameTime, Rectangle clientBounds)
    {
        UpdateRotation();
        base.Update(gameTime, clientBounds);
    }

}