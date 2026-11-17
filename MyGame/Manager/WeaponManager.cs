using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame;

// Connects the player to the weapons.
// Left mouse button -> fire the equipped weapon toward the mouse pointer
// (TEMP) 1 / 2 -> equip the pistol / shotgun
// The shot starts at the tip of the player's gun and aims the way the player
// sprite is already facing. The bullets it creates are handed to the
// BulletManager, which tests them against the enemy sprites.
public class WeaponManager
{
    private readonly List<Weapon> weapons;
    private readonly BulletManager bulletManager;

    // Index into the weapons list of the weapon the player is holding.
    private int currentIndex;

    // False right after gameplay (re)starts, until the mouse button has been let go once.
    // This stops the click that pressed "Start Game" or "Resume" from also firing a shot.
    private bool triggerArmed;

    public Weapon CurrentWeapon => weapons[currentIndex];

    // How many bullets are in the air right now (handy for debugging).
    public int BulletsInFlight => bulletManager.Count;

    public WeaponManager(Texture2D bulletPixel)
    {
        bulletManager = new BulletManager(bulletPixel);

        // The order matters: the weapon at position 0 is key 1, position 1 is key 2.
        weapons = new List<Weapon>
        {
            new Pistol(),
            new Shotgun()
        };
    }

    // Call when gameplay becomes active again (start, resume, restart).
    public void RequireTriggerRelease()
    {
        triggerArmed = false;
    }

    public void Reset()
    {
        bulletManager.Clear();
        currentIndex = 0;

        foreach (Weapon weapon in weapons)
        {
            weapon.ResetCooldown();
            weapon.ResetUpgrades();
        }

        RequireTriggerRelease();
    }

    public void EquipShotgun()
    {
        currentIndex = 1;
    }

    public void SetUpgradeLevels(bool shotgun, int fireRateLevel, int damageLevel)
    {
        int weaponIndex = shotgun ? 1 : 0;
        weapons[weaponIndex].SetUpgradeLevels(fireRateLevel, damageLevel);
    }

    public void Update(
        GameTime gameTime,
        PlayerSprite playerSprite,
        bool canShoot,
        Rectangle screenBounds,
        IReadOnlyList<Enemy> enemies)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Every weapon keeps cooling down, even if it is not the one being held.
        foreach (Weapon weapon in weapons)
        {
            weapon.Update(elapsed);
        }

        // Equip a weapon with the number keys: 1 = first weapon, 2 = second weapon.
        KeyboardState keyboard = Keyboard.GetState();

        for (int i = 0; i < weapons.Count; i++)
        {
            if (keyboard.IsKeyDown(Keys.D1 + i))
                currentIndex = i;
        }

        // Hold the left mouse button to keep firing (the weapon's FireDelay limits the pace).
        bool triggerHeld = Mouse.GetState().LeftButton == ButtonState.Pressed;

        if (!triggerHeld)
            triggerArmed = true;

        if (canShoot && triggerHeld && triggerArmed)
        {
            // The player sprite already turns toward the mouse, so reuse its rotation as the aim.
            float aimAngle = playerSprite.Rotation;

            Vector2 aimDirection = new Vector2(
                (float)Math.Cos(aimAngle),
                (float)Math.Sin(aimAngle));

            // The gun tip is half a sprite-width in front of the player's center.
            float gunLength = playerSprite.FrameSize.X * playerSprite.Scale / 2f;
            Vector2 muzzlePosition = playerSprite.Center + aimDirection * gunLength;

            CurrentWeapon.TryFire(muzzlePosition, aimAngle, bulletManager);
        }

        // Move the bullets and remove any that hit an enemy or the screen boundary.
        bulletManager.Update(gameTime, screenBounds, enemies);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        bulletManager.Draw(spriteBatch);
    }
}