using Microsoft.Xna.Framework;

namespace MyGame.Sprite;

public class Zombie : Enemy
{
    private int health = 100;
    private int max_health = 100;
    private int currency_on_death = 500;

    public Zombie(EnemySprite zombieSprite, Vector2 position, float speed, int health)
        : base(zombieSprite, position, speed, health)
    {
    }

    public override void Hurting()
    {
        //When player bullet hits
        //zombie.health -= damage;
        if (health <= 0)
        {
            //Zombie dies, play death animation, remove from game
            //give currency to player.
        }
    }
}