using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] protected float moveSpeed;
    [SerializeField] public float damage;
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected Rigidbody2D rb;

    void Start()
    {

    }

    protected virtual void Update()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && targetLayerMask == LayerMask.NameToLayer("Enemy"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.gameObject.GetComponent<EnemySoldier>() != null)
                {
                    other.gameObject.GetComponent<EnemySoldier>().TakeDamage(damage);
                }

                if (other.gameObject.GetComponent<EnemyMonster>() != null)
                {
                    //other.gameObject.GetComponent<EnemyMonster>().TakeDamage(damage);
                }
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Tower") && targetLayerMask == LayerMask.NameToLayer("Tower"))
        {
            if (other.gameObject.CompareTag("Tower"))
            {
                if (other.gameObject.GetComponent<Tower>() != null)
                {
                    other.gameObject.GetComponent<Tower>().TakeDamage(damage);
                }
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && targetLayerMask == LayerMask.NameToLayer("Player"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<PlayerController>() != null)
                {
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
                }
            }
        }*/

        if ((targetLayerMask & (1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemySoldier = other.gameObject.GetComponent<EnemySoldier>();
                if (enemySoldier != null)
                {
                    enemySoldier.TakeDamage(damage);
                }

                var enemyMonster = other.gameObject.GetComponent<EnemyMonster>();
                if (enemyMonster != null)
                {
                    enemyMonster.TakeDamage(damage);
                }
            }
            else if (other.gameObject.CompareTag("Tower"))
            {
                var tower = other.gameObject.GetComponent<Tower>();
                if (tower != null)
                {
                    tower.TakeDamage(damage);
                }
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }

            // Destroy the bullet after the collision
            Destroy(gameObject);
        }

        //Destroy(gameObject);
    }
}
