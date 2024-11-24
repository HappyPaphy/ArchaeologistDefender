using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyEntity : CharacterEntity
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public virtual void TakeDamage(float damageValue)
    {
        CharacterHealthComponent.TakeDamage(damageValue);
    }

    protected override void Die()
    {
        GameManager.instance.enemyCount++;
        base.Die();
    }
}
