// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Input;

// public class Player
// {
//     private int health;
//     private int max_health;
//     private float stamina;
//     private float max_stamina;
//     private float damageCooldown = 1.0f;
//     private float damageTimer = 0f;
//     private bool isDead;
//     private int Currency;
//     public PlayerSprite playerSprite {get; private set;}
//     public int Health => health;
//     public int MaxHealth => max_health;
//     public float Stamina => stamina;
//     public float MaxStamina => max_stamina;
//     public bool IsDead => isDead;

//     public Player(PlayerSprite playerSprite, int health, float stamina)
//     {
//         this.playerSprite = playerSprite;
//         this.health = health;
//         this.max_health = health;
//         this.stamina = stamina;
//         this.max_stamina = stamina;
//         this.Currency = 0;
//     }


//     // public virtual void Hurting()
//     // {
//     //     //Subtracts enemy damage from health when the player makes contact with enemy/hazard.
//     //     if (health == 2)
//     //     {
//     //         health -= 10;
//     //         //start a cooldown timer to prevent immediate repeated damage.
//     //     }

//     //     if (health <= 0)
//     //     {
//     //         playerSprite.SetPosition(new Vector2(0, 0));
//     //         //Pause the game and display the "Game Over" screen.
//     //     }
//     // }

//     public void Update(GameTime gameTime)
//     {
//         if (isDead)
//             return;

//         float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

//         // Update damage cooldown.
//         if (damageTimer > 0)
//             damageTimer -= elapsed;

//         KeyboardState keyboard = Keyboard.GetState();

//         bool wantsSprint = keyboard.IsKeyDown(Keys.LeftShift);

//         playerSprite.IsSprinting =
//             wantsSprint &&
//             stamina > 0 &&
//             playerSprite.direction != Vector2.Zero;

//         if (playerSprite.IsSprinting)
//         {
//             stamina -= 25f * elapsed;

//             if (stamina < 0)
//                 stamina = 0;
//         }
//         else
//         {
//             stamina += 15f * elapsed;

//             if (stamina > MaxStamina)
//                 stamina = MaxStamina;
//         }
//     }

//     public void TakeDamage(int damage)
//     {
//         if (isDead || damageTimer > 0)
//             return;

//         health -= damage;

//         damageTimer = damageCooldown;

//         if (health <= 0)
//         {
//             health = 0;
//             Die();
//         }
//     }

//     public bool UseStamina(float amount)
//     {
//         if (stamina < amount || isDead)
//             return false;

//         stamina -= amount;
//         return true;
//     }

//     private void Die()
//     {
//         isDead = true;

//         // Stop the player at their current position.
//         // The game manager can now display a Game Over screen.
//     }

//     public void Respawn(Vector2 spawnPosition)
//     {
//         health = MaxHealth;
//         stamina = MaxStamina;

//         damageTimer = 0;
//         isDead = false;

//         playerSprite.SetPosition(spawnPosition);
//     }
// }


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame.Sprite;

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


