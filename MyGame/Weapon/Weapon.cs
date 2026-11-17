using System;
using Microsoft.Xna.Framework;

namespace MyGame;

// Base class for every weapon.
//
// INFINITE AMMO: weapons deliberately have no ammo count. The only thing that
// limits how often a weapon fires is its FireDelay, so it can never run dry.
//
// RANGE: every bullet a weapon creates travels from the player until it hits
// an enemy or reaches the screen boundary (see BulletManager).
public abstract class Weapon
{
    // Seconds left before the weapon is allowed to fire again.
    private float cooldown;

    // The weapon's base numbers, before any shop upgrade.
    private readonly WeaponStats baseStats;

    // Every upgrade level above 1 adds this fraction of the base damage (level 5 = double damage).
    private const float DamageBonusPerLevel = 0.25f;

    // Shop upgrade level of this weapon (1 = not upgraded).
    public int UpgradeLevel { get; set; } = 1;

    // False until the weapon has been bought in the shop.
    public bool IsUnlocked { get; set; } = true;

    // The numbers bullets are created from: the base stats with the upgrade bonus added to the damage.
    protected WeaponStats Stats => baseStats.WithDamage(
        (int)Math.Round(baseStats.Damage * (1f + DamageBonusPerLevel * (UpgradeLevel - 1))));

    public string Name => baseStats.Name;

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