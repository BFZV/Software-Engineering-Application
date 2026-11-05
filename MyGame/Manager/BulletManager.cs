using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame;

// Keeps the list of every bullet that is currently flying.
// A bullet is removed from the list when it:
//   1. hits an enemy sprite (the enemy takes damage), or
//   2. reaches the screen boundary.
public class BulletManager
{
    // Every bullet that is currently in the air.
    private readonly List<Bullet> bullets = new List<Bullet>();

    // 1x1 white image that bullets stretch into little rectangles.
    private readonly Texture2D pixel;

    // How many bullets are in the air right now.
    public int Count => bullets.Count;

    public BulletManager(Texture2D pixel)
    {
        this.pixel = pixel;
    }

    public void Add(Bullet bullet)
    {
        bullets.Add(bullet);
    }

    public void Update(GameTime gameTime, Rectangle screenBounds, IReadOnlyList<Enemy> enemies)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Walk the list backwards so removing a bullet never makes us skip the next one.
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            Bullet bullet = bullets[i];
            bullet.Update(elapsed);

            // The bullet reached the screen boundary.
            if (bullet.IsOffScreen(screenBounds))
            {
                bullets.RemoveAt(i);
                continue;
            }

            // The bullet touched an enemy sprite.
            Enemy hitEnemy = FindEnemyHitBy(bullet, enemies);

            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(bullet.Damage);
                bullets.RemoveAt(i);
            }
        }
    }

    // Returns the first living enemy the bullet is touching, or null if it touches none.
    private static Enemy FindEnemyHitBy(Bullet bullet, IReadOnlyList<Enemy> enemies)
    {
        Rectangle bulletBounds = bullet.Bounds;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsDead)
                continue;

            if (bulletBounds.Intersects(enemy.Sprite.collisionRect))
                return enemy;
        }

        return null;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Bullet bullet in bullets)
        {
            bullet.Draw(spriteBatch, pixel);
        }
    }
}
