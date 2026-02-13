using UnityEngine;

public class Core : MonoBehaviour, Damageable
{
    [SerializeField] int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            State.Publish(Condition.FINISH);
        }
    }
}
