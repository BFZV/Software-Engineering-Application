using Microsoft.Xna.Framework;

namespace MyGame;

// Base class for every weapon.
//
// RANGE: every bullet a weapon creates travels from the player until it hits
// an enemy or reaches the screen boundary (see BulletManager).
public abstract class Weapon
{
    // Seconds left before the weapon is allowed to fire again.
    private float cooldown;

    protected WeaponStats Stats { get; }

    public string Name => Stats.Name;

    // True when enough time has passed since the last shot.
    public bool IsReady => cooldown <= 0f;

    protected Weapon(WeaponStats stats)
    {
        Stats = stats;
    }

    // Counts the cooldown down. Called every update, even while the weapon is not equipped.
    public void Update(float elapsedSeconds)
    {
        if (cooldown > 0f)
            cooldown -= elapsedSeconds;
    }

    // Fires the weapon if its cooldown is over. Returns true if a shot was fired.
    public bool TryFire(Vector2 muzzlePosition, float aimAngle, BulletManager bullets)
    {
        if (!IsReady)
            return false;

        Fire(muzzlePosition, aimAngle, bullets);
        cooldown = Stats.FireDelay;
        return true;
    }

    // Each weapon decides what one shot looks like (one bullet, a spread of pellets, ...).
    protected abstract void Fire(Vector2 muzzlePosition, float aimAngle, BulletManager bullets);
}
