using System;
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
    private readonly WeaponStats baseStats;

    protected WeaponStats Stats
    {
        get
        {
            float fireRateMultiplier = 1f + 0.25f * (FireRateLevel - 1);
            float damageMultiplier = 1f + 0.25f * (DamageLevel - 1);

            return new WeaponStats(
                baseStats.Name,
                (int)Math.Round(baseStats.Damage * damageMultiplier),
                baseStats.FireDelay / fireRateMultiplier,
                baseStats.BulletSpeed,
                baseStats.BulletSize,
                baseStats.BulletColor);
        }
    }

    public string Name => Stats.Name;
    public int FireRateLevel { get; private set; } = 1;
    public int DamageLevel { get; private set; } = 1;

    // True when enough time has passed since the last shot.
    public bool IsReady => cooldown <= 0f;

    protected Weapon(WeaponStats stats)
    {
        baseStats = stats;
    }

    // Counts the cooldown down. Called every update, even while the weapon is not equipped.
    public void Update(float elapsedSeconds)
    {
        if (cooldown > 0f)
            cooldown -= elapsedSeconds;
    }

    public void ResetCooldown()
    {
        cooldown = 0f;
    }

    public void SetUpgradeLevels(int fireRateLevel, int damageLevel)
    {
        FireRateLevel = Math.Clamp(fireRateLevel, 1, 5);
        DamageLevel = Math.Clamp(damageLevel, 1, 5);
    }

    public void ResetUpgrades()
    {
        SetUpgradeLevels(1, 1);
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
