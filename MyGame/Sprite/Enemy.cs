using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame;

public class Enemy
{
    private int health;
    private int max_health;
    private int damage;
    private int proj_damage;
    private int currency_on_death;

    private Vector2 position;
    private float speed;
    private Vector2 velocity;
    private Vector2 direction;
    private EnemySprite enemySprite;
    public EnemySprite Sprite => enemySprite;
    public int Health => health;
    public bool IsDead => health <= 0;

    public Enemy(EnemySprite enemySprite, Vector2 position, float speed, int health)
    {
        this.enemySprite = enemySprite;
        this.position = position;
        this.speed = speed;
        this.health = health;
        this.max_health = health;
    }

    public virtual void TakeDamage(int amount)
    {
        //When player bullet hits
        //subtract damage from health
        if (IsDead)
            return;

        health -= amount;

        if (health < 0)
            health = 0;
    }

    //Money the player receives when this enemy dies.
    public int CurrencyOnDeath { get; protected set; }
}