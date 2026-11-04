using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.Sprite;

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

    public Enemy(EnemySprite enemySprite, Vector2 position, float speed, int health)
    {
        this.enemySprite = enemySprite;
        this.position = position;
        this.speed = speed;
        this.health = health;
        this.max_health = health;
    }


    public virtual void Hurting()
    {
        //When player bullet hits
        //subtract damage from health
    }


}