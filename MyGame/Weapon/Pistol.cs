using Microsoft.Xna.Framework;

namespace MyGame;

// Fast, accurate weapon: one bullet per shot.
public class Pistol : Weapon
{
    public Pistol() : base(new WeaponStats(
        name: "Pistol",
        damage: 20,
        fireDelay: 0.3f,
        bulletSpeed: 1600f,
        bulletSize: new Vector2(32f, 8f),
        bulletColor: Color.Yellow))
    {
    }

    protected override void Fire(Vector2 muzzlePosition, float aimAngle, BulletManager bullets)
    {
        bullets.Add(new Bullet(muzzlePosition, aimAngle, Stats));
    }
}
