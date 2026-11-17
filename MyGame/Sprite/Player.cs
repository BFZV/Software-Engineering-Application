 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame;

public class Player
{
    private int health;
    private int maxHealth;

    private float stamina;
    private float maxStamina;

    private const float SprintCost = 50f;
    private const float StaminaRecovery = 50f;

    private const float DamageCooldown = 1.0f;
    private const float RespawnDelay = 3.0f;
    private const float RespawnInvulnerability = 1.5f;

    private float damageTimer;
    private float respawnTimer;
    private float invulnerabilityTimer;

    private bool isDead;

    public PlayerSprite Sprite { get; private set; }

    public int Health => health;
    public int MaxHealth => maxHealth;

    public float Stamina => stamina;
    public float MaxStamina => maxStamina;

    public bool IsDead => isDead;

    public Player(
        PlayerSprite playerSprite,
        int health,
        float stamina)
    {
        Sprite = playerSprite;

        this.health = health;
        maxHealth = health;

        this.stamina = stamina;
        maxStamina = stamina;
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (isDead)
        {
            respawnTimer -= elapsed;

            if (respawnTimer <= 0)
            {
                Respawn(new Vector2(50, 50));
            }

            return;
        }

        // Update timers.
        if (damageTimer > 0)
            damageTimer -= elapsed;

        if (invulnerabilityTimer > 0)
            invulnerabilityTimer -= elapsed;

        // Check sprint input.
        KeyboardState keyboard = Keyboard.GetState();

        bool wantsSprint =
            keyboard.IsKeyDown(Keys.LeftShift);

        bool isMoving =
            //Sprite.direction != Vector2.Zero;
            keyboard.IsKeyDown(Keys.W) ||
            keyboard.IsKeyDown(Keys.A) ||
            keyboard.IsKeyDown(Keys.S) ||
            keyboard.IsKeyDown(Keys.D);

        // Sprint only if stamina is available.
        Sprite.IsSprinting =
            wantsSprint &&
            isMoving &&
            stamina > 0;

        // Consume or regenerate stamina.
        if (Sprite.IsSprinting)
        {
            stamina -= SprintCost * elapsed;

            if (stamina <= 0)
            {
                stamina = 0;
                Sprite.IsSprinting = false;
            }
        }
        else
        {
            stamina += StaminaRecovery * elapsed;

            if (stamina > maxStamina)
                stamina = maxStamina;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead ||
            damageTimer > 0 ||
            invulnerabilityTimer > 0)
            return;

        health -= damage;

        damageTimer = DamageCooldown;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void RestoreHealth()
    {
        health = maxHealth;
    }

    private void Die()
    {
        isDead = true;

        Sprite.IsSprinting = false;
        respawnTimer = RespawnDelay;
    }

    public void Respawn(Vector2 spawnPosition)
    {
        health = maxHealth;
        stamina = maxStamina;

        damageTimer = 0;
        respawnTimer = 0;
        invulnerabilityTimer = RespawnInvulnerability;

        isDead = false;

        Sprite.IsSprinting = false;
        Sprite.SetPosition(spawnPosition);
    }
}


