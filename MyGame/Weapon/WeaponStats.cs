using Microsoft.Xna.Framework;

namespace MyGame;

// Plain data that describes how a weapon behaves.
// A struct is enough here because the numbers never change after the weapon is created.
public readonly struct WeaponStats
{
    // Name shown in the weapon HUD.
    public string Name { get; }

    // Damage dealt by ONE bullet (or one shotgun pellet).
    public int Damage { get; }

    // Seconds that must pass between two shots.
    public float FireDelay { get; }

    // How fast a bullet travels, in pixels per second.
    public float BulletSpeed { get; }

    // Size a bullet is drawn at: X is the length, Y is the thickness.
    public Vector2 BulletSize { get; }

    // Color a bullet is drawn with.
    public Color BulletColor { get; }

    public WeaponStats(
        string name,
        int damage,
        float fireDelay,
        float bulletSpeed,
        Vector2 bulletSize,
        Color bulletColor)
    {
        Name = name;
        Damage = damage;
        FireDelay = fireDelay;
        BulletSpeed = bulletSpeed;
        BulletSize = bulletSize;
        BulletColor = bulletColor;
    }
}
