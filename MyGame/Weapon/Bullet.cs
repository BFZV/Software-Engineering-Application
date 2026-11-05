using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame;

// One bullet (or one shotgun pellet) flying in a straight line.
// A bullet has no range limit of its own: it keeps flying until the
// BulletManager removes it for hitting an enemy or reaching the screen edge.
public class Bullet
{
    // Side length of the small square used to test whether the bullet touches an enemy.
    private const int HitboxSize = 8;

    // Pixels per second, already split into an X part and a Y part.
    private readonly Vector2 velocity;

    // Angle the bullet is pointing, used only when drawing it.
    private readonly float rotation;

    // Length and thickness the bullet is drawn at.
    private readonly Vector2 drawSize;

    private readonly Color color;

    // Center of the bullet on screen.
    public Vector2 Position { get; private set; }

    // How much health an enemy loses when this bullet hits it.
    public int Damage { get; }

    // Small box around the bullet used for enemy collisions.
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - HitboxSize / 2,
        (int)Position.Y - HitboxSize / 2,
        HitboxSize,
        HitboxSize);

    public Bullet(Vector2 position, float angle, WeaponStats stats)
    {
        Position = position;
        rotation = angle;

        // Turn the angle into a direction, then multiply by the weapon's bullet speed.
        velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * stats.BulletSpeed;

        Damage = stats.Damage;
        drawSize = stats.BulletSize;
        color = stats.BulletColor;
    }

    public void Update(float elapsedSeconds)
    {
        Position += velocity * elapsedSeconds;
    }

    // True once the center of the bullet has left the screen.
    public bool IsOffScreen(Rectangle screenBounds)
    {
        return !screenBounds.Contains(Position);
    }

    // Draws the bullet by stretching a 1x1 white pixel into a small rotated rectangle.
    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(
            pixel,
            Position,
            null,
            color,
            rotation,
            new Vector2(0.5f, 0.5f),
            drawSize,
            SpriteEffects.None,
            0f);
    }
}
