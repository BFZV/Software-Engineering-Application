using Microsoft.Xna.Framework;

namespace MyGame;

public class Zombie : Enemy
{
    private int health = 100;
    private int max_health = 100;
    private int currency_on_death = 500;

    public Zombie(EnemySprite zombieSprite, Vector2 position, float speed, int health)
        : base(zombieSprite, position, speed, health)
    {
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (IsDead)
        {
            //Zombie dies, play death animation
            //give currency to player.
            //(SpriteManager removes it from the game.)
        }
    }
}