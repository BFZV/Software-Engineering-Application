using System;
using Microsoft.Xna.Framework;

namespace MyGame;

// Slow, powerful weapon: one shot fires a fan of pellets.
// Each pellet is its own Bullet, so a close enemy can be hit by several of them.
public class Shotgun : Weapon
{
    // How many pellets leave the barrel in one shot.
    private const int PelletCount = 8;

    // Total width of the fan of pellets, in degrees.
    private const float SpreadDegrees = 24f;

    // Small random wobble added to each pellet, in degrees, so the fan is not perfectly even.
    private const float JitterDegrees = 1.5f;

    private readonly Random random = new Random();

    public Shotgun() : base(new WeaponStats(
        name: "Shotgun",
        damage: 12,
        fireDelay: 1.0f,
        bulletSpeed: 1300f,
        bulletSize: new Vector2(32f, 8f),
        bulletColor: Color.Yellow))
    {
    }

    protected override void Fire(Vector2 muzzlePosition, float aimAngle, BulletManager bullets)
    {
        float spread = MathHelper.ToRadians(SpreadDegrees);
        float jitter = MathHelper.ToRadians(JitterDegrees);

        // Angle between two neighbouring pellets.
        float step = spread / (PelletCount - 1);

        for (int i = 0; i < PelletCount; i++)
        {
            // Start at the left edge of the fan and move right one step per pellet.
            float angle = aimAngle - spread / 2f + step * i;

            // Add a random wobble between -jitter and +jitter.
            angle += ((float)random.NextDouble() * 2f - 1f) * jitter;

            bullets.Add(new Bullet(muzzlePosition, angle, Stats));
        }
    }
}
