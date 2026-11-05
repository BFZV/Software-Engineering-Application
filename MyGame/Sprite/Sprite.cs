using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame;

public abstract class Sprite
{
    protected Texture2D _textureImage;
    public Point frameSize;
    protected Point currentFrame;
    protected Point sheetSize;
    protected int collisionOffset = 3;
    protected int timeSinceLastFrame = 0;
    protected int millisecondsPerFrame;
    protected const int defaultMillisecondsPerFrame = 16;
    protected Vector2 speed;
    protected Vector2 position;
    protected float rotation = 0f;
    protected float scale = 1f;
    public Vector2 Position => position;
    public Point FrameSize => frameSize;
    public float Scale => scale;
    public float Rotation => rotation;
    // Middle of the sprite on screen (position is the top-left corner).
    public Vector2 Center => position + new Vector2(frameSize.X, frameSize.Y) * scale / 2f;

    public Sprite(
        Texture2D textureImage,
        Vector2 position,
        Point frameSize,
        int collisionOffset,
        Point currentFrame,
        Point sheetSize,
        Vector2 speed)
        : this(textureImage, position, frameSize, collisionOffset,
               currentFrame, sheetSize, speed,
               defaultMillisecondsPerFrame)
    {
    }

    public Sprite(
        Texture2D textureImage,
        Vector2 position,
        Point frameSize,
        int collisionOffset,
        Point currentFrame,
        Point sheetSize,
        Vector2 speed,
        int millisecondsPerFrame)
    {
        _textureImage = textureImage;
        this.position = position;
        this.frameSize = frameSize;
        this.collisionOffset = collisionOffset;
        this.currentFrame = currentFrame;
        this.sheetSize = sheetSize;
        this.speed = speed;
        this.millisecondsPerFrame = millisecondsPerFrame;
    }

    public void SetPosition(Vector2 newPosition)
    {
        position = newPosition;
    }
    
    public virtual void Update(GameTime gameTime, Rectangle clientBounds)
    {
        timeSinceLastFrame +=
            gameTime.ElapsedGameTime.Milliseconds;

        if (timeSinceLastFrame >= millisecondsPerFrame)
        {
            timeSinceLastFrame = 0;
            ++currentFrame.X;

            if (currentFrame.X >= sheetSize.X)
            {
                currentFrame.X = 0;
                ++currentFrame.Y;

                if (currentFrame.Y >= sheetSize.Y)
                    currentFrame.Y = 0;
            }
        }
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Rectangle sourceRectangle = new Rectangle(
            currentFrame.X * frameSize.X,
            currentFrame.Y * frameSize.Y,
            frameSize.X,
            frameSize.Y
        );

        Vector2 origin = new Vector2(
            frameSize.X / 2f,
            frameSize.Y / 2f
        );

        Vector2 drawPosition = position + origin * scale;

        spriteBatch.Draw(
            _textureImage,
            drawPosition,
            sourceRectangle,
            Color.White,
            rotation,
            origin,
            scale,
            SpriteEffects.None,
            0f
        );
    }

    public abstract Vector2 direction { get; }

    public Rectangle collisionRect
    {
        get
        {
            return new Rectangle(
                (int)position.X + (int)(collisionOffset * scale),
                (int)position.Y + (int)(collisionOffset * scale),
                (int)((frameSize.X - collisionOffset * 2) * scale),
                (int)((frameSize.Y - collisionOffset * 2) * scale)
            );
        }
    }
}